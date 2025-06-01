import { use, useEffect, useState } from "react";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import NavMenu from "../NavMenu"
import useToken from '../useToken';
import { Button, Card, CardBody, CardHeader, Input, Label } from "reactstrap";
import { ApiError, ClientJobRequest, Conversation, findTradesMan, FindTradesMan, getConversation, getConversations, getGlobalRequests, getMessages, MessageOrResponse, sendMessage } from "@/api";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { Switch } from "../ui/switch";
import { useUser } from "@/context/UserContext";

type ConversationOrGlobalRequest = { conversation: Conversation, globalRequest: undefined } | { globalRequest: ClientJobRequest, conversation: undefined };

export default function () {
    const [conversations, setConversations] = useState<Conversation[]>([]);
    const [globalRequests, setGlobalRequests] = useState<ClientJobRequest[]>([]);
    const [combinedConversations, setCombinedConversations] = useState<ConversationOrGlobalRequest[]>([]);
    const [selectedConversation, setSelectedConversation] = useState<ConversationOrGlobalRequest | null>(null);

    const [messages, setMessages] = useState<MessageOrResponse[] | null>(null);

    const [dialogOpen, setDialogOpen] = useState(false);
    const [newUserName, setNewUserName] = useState("");
    const [usersSuggestions, setUsersSuggestions] = useState<FindTradesMan[] | null>([]);
    const [includeGlobalRequests, setIncludeGlobalRequests] = useState(false);
    const { token } = useToken();
    const { user } = useUser();
    const loadConversations = async (signal?: AbortSignal) => {
        try {
            const convs = await getConversations(token, signal);
            setConversations(convs);
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
    }

    const loadGlobalRequests = async (signal?: AbortSignal) => {
        try {
            const requests = await getGlobalRequests(token, signal);
            setGlobalRequests(requests);
        } catch (error) {
            if (error instanceof ApiError) {
                if (error.error.type !== "aborted") {
                    toast(error.message)
                }
            }
        }
    }

    useEffect(() => {
        let controller = new AbortController();
        loadConversations(controller.signal);

        return () => {
            controller.abort();
        };
    }, []);

    useEffect(() => {
        const combined = [...conversations.map(c => ({ conversation: c, globalRequest: undefined })),
        ...globalRequests.map(req => ({ globalRequest: req, conversation: undefined }))];
        setCombinedConversations(combined.map(item => ({ ...item })));
    }, [conversations, globalRequests]);



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

    useEffect(() => {
        let controller = new AbortController();
        if (!includeGlobalRequests) {
            setGlobalRequests([]);
            return;
        }
        loadGlobalRequests(controller.signal);

        return () => {
            controller.abort();
        }
    }, [includeGlobalRequests]);

    useEffect(() => {
        if (!selectedConversation) return;
        if (selectedConversation.globalRequest && !includeGlobalRequests) {
            setSelectedConversation(null);
        }
    }, [selectedConversation, includeGlobalRequests])

    useEffect(() => {
        setMessages(null);
        if (!selectedConversation) {
            return;
        }
        if (!selectedConversation.conversation) {
            setMessages([]);
            return;
        }
        let controller = new AbortController();
        (async () => {
            const messages = await getMessages(selectedConversation.conversation.id, token, controller.signal);
            setMessages(messages);
        })();
        return () => {
            controller.abort();
        }
    }, [selectedConversation]);

    const [message, setMessage] = useState("");

    const onSendMessage = () => {
        if (!message.trim() || !selectedConversation) return;
        (async () => {
            try {
                let conversation;
                if (selectedConversation.globalRequest) {
                    const conversationResult = await getConversation(selectedConversation.globalRequest.id, token);
                    loadConversations();
                    loadGlobalRequests();
                    conversation = conversationResult;
                    console.log("Am create conversatia");
                    console.log(conversation);
                } else {
                    conversation = selectedConversation.conversation;
                }
                const r = await sendMessage({
                    conversationId: conversation.id,
                    text: message
                }, token);
                console.log(r);

                const messageResponse: MessageOrResponse = {
                    isMe: true,
                    sent: r.sent,
                    message: {
                        from: r.from,
                        text: r.text,
                        id: r.id,
                    },
                    response: undefined,
                };
                setMessages((prev) => {
                    if (!prev) return [messageResponse];
                    return [...prev, messageResponse];
                });
                setMessage("");
            } catch (error) {
                if (error instanceof ApiError) {
                    if (error.error.type !== "aborted") {
                        console.log(error.error);
                        toast.error(`${error.message}`);
                    }
                    return;
                } else {
                    throw error;
                }
            }
        })();
    };

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


    return (
        <div>
            <NavMenu />
            <ToastContainer />
            <div className="flex h-screen bg-gray-100">

                {/* Conversations List */}
                <div className="w-1/3 border-r bg-white p-4 overflow-y-auto">
                    {user?.isTradesman && <div className="flex justify-left mb-4 center">
                        <Switch checked={includeGlobalRequests} onCheckedChange={setIncludeGlobalRequests}></Switch>
                        <Label>Include global requests</Label>
                    </div>}
                    {combinedConversations.map((conv) => (
                        <Card
                            key={conv.conversation?.id || conv.globalRequest?.id}
                            className={`mb-2 cursor-pointer hover:bg-gray-100 transition duration-300 bg-gray-200
                                }`}
                            onClick={() => setSelectedConversation(conv)}
                        >
                            {conv.conversation && <div>
                                <CardHeader>{conv.conversation.clientRequest.title}</CardHeader>
                                <CardBody className="p-4">
                                    <p className="font-semibold">{conv.conversation.tradesMan.name}</p>
                                    {conv.conversation.lastMessage && <p className="text-sm text-gray-600">
                                        {conv.conversation.lastMessage.isMe ? "You: " : ""} {conv.conversation.lastMessage.message ? conv.conversation.lastMessage.message.text : `${conv.conversation.lastMessage.response.workmanshipAmount} by ${conv.conversation.lastMessage.response.AproximationEndDate}`}
                                    </p>}
                                </CardBody>
                            </div>}
                            {conv.globalRequest && <div>
                                <CardHeader>{conv.globalRequest.title}</CardHeader>
                                <CardBody className="p-4">
                                    Global
                                </CardBody>
                            </div>}
                        </Card>
                    ))}
                </div>

                {/* Message Panel */}
                {selectedConversation && <div className="flex-1 flex flex-col">
                    <div className="flex-1 overflow-y-auto p-6 bg-gradient-to-br from-white to-blue-50">
                        <div className="space-y-4">
                            {messages ? "" : "Loading messages..."}
                            {messages?.map((msg) => (
                                <div
                                    key={msg.message ? msg.message.id : msg.response.id}
                                    className={`flex ${msg.isMe ? "justify-end" : "justify-start"
                                        }`}
                                >
                                    {msg.message ?
                                        <div
                                            className={`max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm ${msg.isMe
                                                ? "bg-blue-500 rounded-br-none"
                                                : "bg-gray-400 rounded-bl-none"
                                                }`}
                                        >
                                            {msg.message.text}
                                        </div> :
                                        <div
                                            className={`max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm ${msg.isMe
                                                ? "bg-blue-500 rounded-br-none"
                                                : "bg-gray-400 rounded-bl-none"
                                                }`}
                                        >
                                            <h4>Proposed resolution</h4>
                                            <p>Can be done by {msg.response.AproximationEndDate}</p>
                                            <p>The workmanship will be {msg.response.workmanshipAmount}</p>
                                            <Button
                                                className="mt-2"
                                                onClick={() => {
                                                    // Handle job acceptance logic here
                                                    toast.success("Job accepted!");
                                                }}>Accept proposal</Button>
                                        </div>
                                    }

                                </div>
                            ))}
                        </div>
                    </div>

                    <div className="p-4 border-t bg-white flex gap-2">
                        <Input
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            placeholder="Type a message..."
                            className="flex-1"
                            onKeyDown={(e) => e.key === "Enter" && onSendMessage()}
                        />
                        <Button onClick={onSendMessage}>Send</Button>
                    </div>
                </div>
                }

                {/* New Conversation Dialog */}
                <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Start a New Conversation</DialogTitle>
                        </DialogHeader>
                        <Input
                            value={newUserName}
                            onChange={(e) => setNewUserName(e.target.value)}
                            placeholder="Enter user name"
                            className="my-4"
                        />
                        <ul>
                            {usersSuggestions?.map((user) => (
                                <li
                                    key={`Sugested ${user.id}`}
                                    className="cursor-pointer hover:bg-gray-100 p-2 rounded"
                                    onClick={() => onClickUserSuggestion(user)}
                                >
                                    {user.imageUrl && <img src={user.imageUrl} alt={user.name} className="inline-block w-6 h-6 rounded-full ml-2" />}
                                    {user.name}
                                </li>
                            ))}
                        </ul>
                    </DialogContent>
                </Dialog>
            </div>
        </div >
    );
}