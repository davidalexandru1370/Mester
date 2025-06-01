import { useEffect, useState } from "react";
import axios from "axios";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import NavMenu from "../NavMenu"
import useToken from '../useToken';
import { Button, Card, CardBody, CardHeader, Input, Label } from "reactstrap";
import { ApiError, clientGetRequests, clientGetRequestsConversations, ClientJobRequest, ClientJobResponsesConversation, Conversation, findTradesMan, FindTradesMan, getConversation, getConversations, getMessages, Message, MessageOrResponse, sendMessage } from "@/api";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { Plus } from "lucide-react";
import { Checkbox } from "../ui/checkbox";
import RequestDetails from "./RequestDetails";


export default function () {
    const [clientRequests, setClientRequests] = useState<ClientJobRequest[]>([]);

    const [tradesManResponses, setTradesManResponses] = useState<ClientJobResponsesConversation[] | null>(null);

    const [dialogOpen, setDialogOpen] = useState(false);
    const [newUserName, setNewUserName] = useState("");
    const [usersSuggestions, setUsersSuggestions] = useState<FindTradesMan[] | null>([]);
    const [selectedRequest, setSelectedRequest] = useState<ClientJobRequest | null>(null);

    const handleImageChange = (request: ClientJobRequest, index: number, value: string) => {
        const updatedImages = [...request.imagesUrl];
        updatedImages[index] = value;
        setSelectedRequest({ ...request, imagesUrl: updatedImages });
    };

    // const addImage = () => {
    //     handleChange("imagesUrl", [...formData.imagesUrl, ""]);
    // };

    const removeImage = (request: ClientJobRequest, index: number) => {
        const updatedImages = request.imagesUrl.filter((_, i) => i !== index);
        setSelectedRequest({ ...request, imagesUrl: updatedImages });
    };

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
            const requests = await clientGetRequests(token, controller.signal);
            setClientRequests(requests);
        })();

        return () => {
            controller.abort();
        }
    }, []);


    useEffect(() => {
        setTradesManResponses(null);
        if (!selectedRequest) {
            return;
        }
        let controller = new AbortController();
        (async () => {
            // TODO: fetch details page
            const messages = await clientGetRequestsConversations(selectedRequest.id, token, controller.signal);
            setTradesManResponses(messages);
        })();
        return () => {
            controller.abort();
        }
    }, [selectedRequest]);

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

    const [updateButtonDisabled, setUpdateButtonDisabled] = useState(false);
    const onUpdateRequest = (request: ClientJobRequest) => async () => {
        setUpdateButtonDisabled(true);


        setUpdateButtonDisabled(false);
    }

    return (
        <div>
            <NavMenu />
            <div className="flex h-screen bg-gray-100">
                {/* Conversations List */}
                <div className="w-1/3 border-r bg-white p-4 overflow-y-auto">
                    <Button size="icon" variant="outline" onClick={() => setDialogOpen(true)}>
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
                    <RequestDetails initialRequestDetails={selectedRequest} onUpdateRequestDetails={r => { }} ></RequestDetails>
                    <div className="flex-1 overflow-y-auto p-6 bg-gradient-to-br from-white to-blue-50">

                        <div className="space-y-4">
                            <div>
                                <Label>Requested On {selectedRequest.requestedOn}</Label>
                            </div>

                            <div>
                                <Label>Start Date</Label>
                                <Input
                                    type="date"
                                    value={selectedRequest.startDate || ""}
                                    onChange={(e) => {
                                        setSelectedRequest({ ...selectedRequest, startDate: e.target.value })
                                    }}
                                />
                            </div>

                            <div>
                                <Label>Title</Label>
                                <Input
                                    value={selectedRequest.title}
                                    onChange={(e) => setSelectedRequest({ ...selectedRequest, title: e.target.value })}
                                />
                            </div>

                            <div>
                                <Label>Description</Label>
                                <textarea
                                    value={selectedRequest.description}
                                    onChange={(e) => setSelectedRequest({ ...selectedRequest, description: e.target.value })}
                                />
                            </div>

                            <div className="flex items-center gap-2">
                                <Checkbox
                                    checked={selectedRequest.showToEveryone}
                                    onCheckedChange={(val) => {
                                        if (val !== "indeterminate")
                                            setSelectedRequest({ ...selectedRequest, showToEveryone: val })
                                    }
                                    }
                                />
                                <Label>Show to Everyone</Label>
                            </div>

                            <div className="flex items-center gap-2">
                                <Checkbox
                                    checked={selectedRequest.open}
                                    onCheckedChange={(val) => {
                                        if (val !== "indeterminate")
                                            setSelectedRequest({ ...selectedRequest, open: val })
                                    }}
                                />
                                <Label>Open</Label>
                            </div>

                            <div className="space-y-2">
                                <Label>Images</Label>
                                {selectedRequest.imagesUrl.map((url, index) => (
                                    <div key={index} className="flex items-center gap-2">
                                        <Input
                                            value={url}
                                            onChange={(e) => handleImageChange(selectedRequest, index, e.target.value)}
                                        />
                                        <Button variant="destructive" onClick={() => removeImage(selectedRequest, index)}>
                                            Remove
                                        </Button>
                                    </div>
                                ))}
                                <div className="p-4 border-t bg-white flex gap-2">
                                    <Button onClick={onUpdateRequest(selectedRequest)} disabled={updateButtonDisabled}>Update request</Button>
                                </div>
                                {/*
                                TODO: add this feature
                                 <Button variant="outline" onClick={addImage}>
                                    Add Image
                                </Button> */}
                            </div>
                        </div>
                        <div className="space-y-4">
                            {tradesManResponses ? "" : "Loading conversations..."}
                            {tradesManResponses?.map((response) => (
                                <div
                                    key={response.id}
                                    className="flex justify-start"
                                    onClick={() => {
                                        // TODO: navigate to the conversation with that user
                                    }}
                                >
                                    <p>{response.tradesMan.name}</p>
                                    {response.response &&
                                        <div
                                            className={"max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm bg-gray-400 rounded-bl-none"}
                                        >
                                            <h4>Proposed resolution</h4>
                                            <p>Can be done by {response.response.AproximationEndDate}</p>
                                            <p>The workmanship will be {response.response.workmanshipAmount}</p>
                                        </div>
                                    }
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
                }

                {/* New Conversation Dialog */}
                <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                    <DialogContent>
                        <RequestDetails onUpdateRequestDetails={r => { console.log(r) }}></RequestDetails>
                    </DialogContent>
                </Dialog>
            </div>
        </div>
    );
}