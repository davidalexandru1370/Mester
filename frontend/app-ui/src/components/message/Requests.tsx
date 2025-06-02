import { useEffect, useState } from "react";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import NavMenu from "../NavMenu"
import useToken from '../useToken';
import { Button, Card, CardBody, CardHeader, } from "reactstrap";
import { ApiError, clientGetRequests, ClientJobRequest, ClientJobRequestCreateOrUpdate, createRequest, updateRequest } from "@/api";
import { Dialog, DialogContent, } from "../ui/dialog";
import { Plus } from "lucide-react";
import RequestDetails from "./RequestDetails";


export default function () {
    const [clientRequests, setClientRequests] = useState<ClientJobRequest[]>([]);


    const [dialogOpen, setDialogOpen] = useState(false);
    const [selectedRequest, setSelectedRequest] = useState<ClientJobRequest | null>(null);
    const [disableUpdateButton, setDisableUpdateButton] = useState(false);



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

    const createOrUpdateRequest = async (request: ClientJobRequestCreateOrUpdate) => {
        setDisableUpdateButton(true);
        try {
            if (request.id) {
                // Just why, typescript?
                const updatedResponse = await updateRequest({ ...request, id: request.id }, token);
                let response = clientRequests.map(r => {
                    if (r.id === updatedResponse.id) {
                        return updatedResponse;
                    } else {
                        return r
                    }
                });
                setSelectedRequest(updatedResponse);
                setClientRequests(response);
                console.log("Request updated successfully.");
                toast.success("Request updated successfully.");
            } else {
                const r = await createRequest(request, token);
                setSelectedRequest(r);
                setClientRequests([r, ...clientRequests])
                console.log("Request created successfully.");
                toast.success("Request created successfully.");
            }
        } catch (error) {
            if (error instanceof ApiError) {
                if (error.error.type !== "aborted") {
                    console.log(error.error);
                    toast.error(`${error.message}`);
                }
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
                    <Button size="icon" variant="outline" onClick={() => setDialogOpen(true)} style={{ marginBottom: 10, }}>
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
                {selectedRequest && <div className="flex-1 flex flex-col overflow-y-auto max-h-screen p-4">
                    <RequestDetails
                        initialRequestDetails={selectedRequest}
                        disableUpdateButton={disableUpdateButton}
                        onUpdateRequestDetails={(r) => createOrUpdateRequest(r)}
                        editable={true}
                    />
                </div>}


                {/* New Conversation Dialog */}
                <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                    <DialogContent>
                        <RequestDetails editable={true} disableUpdateButton={disableUpdateButton} onUpdateRequestDetails={r => {
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