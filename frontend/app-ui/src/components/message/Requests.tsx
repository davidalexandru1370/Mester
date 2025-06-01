import { useEffect, useState } from "react";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import NavMenu from "../NavMenu"
import useToken from '../useToken';
import { Button, Card, CardBody, CardHeader, } from "reactstrap";
import { ApiError, clientGetRequests, ClientJobRequest, ClientJobRequestWithoutId, createRequest, findTradesMan, FindTradesMan, updateRequest } from "@/api";
import { Dialog, DialogContent, } from "../ui/dialog";
import { Plus } from "lucide-react";
import RequestDetails from "./RequestDetails";


export default function () {
    const [clientRequests, setClientRequests] = useState<ClientJobRequest[]>([]);


    const [dialogOpen, setDialogOpen] = useState(false);
    const [newUserName, setNewUserName] = useState("");
    const [usersSuggestions, setUsersSuggestions] = useState<FindTradesMan[] | null>([]);
    const [selectedRequest, setSelectedRequest] = useState<ClientJobRequest | null>(null);
    const [disableUpdateButton, setDisableUpdateButton] = useState(false);
    const handleImageChange = (request: ClientJobRequest, index: number, value: string) => {
        const updatedImages = [...request.imagesUrl];
        updatedImages[index] = value;
        setSelectedRequest({ ...request, imagesUrl: updatedImages });
    };

    // const addImage = () => {
    //     handleChange("imagesUrl", [...formData.imagesUrl, ""]);
    // };

    useEffect(() => {
        if (!dialogOpen) {
            setUsersSuggestions(null);
            return;
        }
        let controller = new AbortController();
        (async () => {
            const trimmedUserName = newUserName.trim();
            try {
                const suggestions = await findTradesMan({
                    pattern: trimmedUserName,
                    limit: 10
                }, token, controller.signal);
                setUsersSuggestions(suggestions);
            } catch (error) {
                if (error instanceof ApiError) {
                    if (error.error.type !== "aborted") {
                        console.log(error);
                        toast.error(`${error.message}`);
                    }
                    return;
                } else {
                    throw error;
                }
            }
        })();


        return () => {
            controller.abort();
        };
    }, [dialogOpen, newUserName]);

    const { token } = useToken();
    useEffect(() => {
        let controller = new AbortController();
        (async () => {
            try {
                const requests = await clientGetRequests(token, controller.signal);
                setClientRequests(requests);
            }
            catch (error) {
                if (error instanceof ApiError) {
                    if (error.error.type !== "aborted") {
                        console.log(error);
                        toast.error(`${error.message}`);
                    }
                    return;
                } else {
                    throw error;
                }
            }
        })();

        return () => {
            controller.abort();
        }
    }, []);

    const onClickUserSuggestion = (user: FindTradesMan) => {
        setDialogOpen(false);
        setNewUserName("");
        // (async () => {
        //     try {
        //         const conversation = await getConversation(user.id, token);
        //         setConversations([conversation, ...conversations]);
        //         setSelectedConversation(conversation);
        //     } catch (error) {
        //         if (error instanceof ApiError) {
        //             toast.error(`${error.message}`);
        //         }
        //     }
        // })();
    }

    const createOrUpdateRequest = async (request: ClientJobRequestWithoutId) => {
        setDisableUpdateButton(true);
        try {
            if (request.id) {
                // Just why, typescript?
                await updateRequest({ ...request, id: request.id }, token);
                let response = clientRequests.map(r => {
                    if (r.id === request.id) {
                        return { ...request, id: request.id };
                    } else {
                        return r
                    }
                });
                setClientRequests(response);
                toast.success("Request updated successfully.");
            } else {
                const r = await createRequest(request, token);
                setClientRequests([r, ...clientRequests])
                toast.success("Request created successfully.");
            }
        } catch (error) {
            if (error instanceof ApiError) {
                console.log(error);
                toast.error(`${error.message}`);
            } else {
                console.log("Unexpected error:", error);
                toast.error("An unexpected error occurred.");
            }
            return;
        } finally {
            setDisableUpdateButton(false);
        }
    }
    return (
        <div>
            <NavMenu />
            <ToastContainer />
            <div className="flex h-screen bg-gray-100">
                {/* Conversations List */}
                <div className="w-1/3 border-r bg-white p-4 overflow-y-auto">
                    <Button size="icon" variant="outline" onClick={() => setDialogOpen(true)} style={{marginBottom: 10,}}>
                        <Plus className="w-5 h-5" />
                    </Button>
                    {clientRequests.map((request) => (
                        <Card
                            key={request.id}
                            className={`mb-2 cursor-pointer hover:bg-gray-100 transition duration-300 ${selectedRequest?.id === request.id ? "bg-gray-200" : ""
                                }`}
                            onClick={() => setSelectedRequest(request)}
                        >
                            <CardHeader>{request.title}</CardHeader>
                            <CardBody className="p-4">
                                <p className="font-semibold">{request.description}</p>
                                {/* {request.lastMessage && <p className="text-sm text-gray-600">
                                    {request.lastMessage.isMe ? "You: " : ""} {request.lastMessage.type === "message" ? request.lastMessage.text : `${request.lastMessage.workmanshipAmount} by ${request.lastMessage.AproximationEndDate}`}
                                </p>} */}
                            </CardBody>
                        </Card>
                    ))}
                </div>

                {/* Message Panel */}
                {selectedRequest && <div className="flex-1 flex flex-col">
                    <RequestDetails initialRequestDetails={selectedRequest} disableUpdateButton={disableUpdateButton} onUpdateRequestDetails={r => createOrUpdateRequest(r)} /> </div>}


                {/* New Conversation Dialog */}
                <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                    <DialogContent>
                        <RequestDetails disableUpdateButton={disableUpdateButton} onUpdateRequestDetails={r => {
                            (async () => {
                                await createOrUpdateRequest(r)
                                setDialogOpen(false);
                            })()
                        }}></RequestDetails>
                    </DialogContent>
                </Dialog>
            </div>
        </div>
    );
}