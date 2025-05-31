import { useEffect, useState } from "react";
import axios from "axios";
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import NavMenu from "../NavMenu"
import useToken from '../useToken';
import { Button, Card, CardBody, Input } from "reactstrap";
import { ApiError, Conversation, findTradesMan, FindTradesMan, getConversation, getConversations, getMessages, Message, sendMessage } from "@/api";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { Plus } from "lucide-react";



export default function () {
    const [conversations, setConversations] = useState<Conversation[]>([]);

    const [messages, setMessages] = useState<Message[] | null>(null);

    const [dialogOpen, setDialogOpen] = useState(false);
    const [newUserName, setNewUserName] = useState("");
    const [usersSuggestions, setUsersSuggestions] = useState<FindTradesMan[] | null>([]);

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
                if (axios.isCancel(error)) {
                    console.log("Request canceled", error.message);
                } else {
                    toast.error("Failed to fetch user suggestions");
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
            const conversations = await getConversations(token, controller.signal);
            setConversations(conversations);
            console.log(conversations);
        })();

        return () => {
            controller.abort();
        }
    }, []);

    const [selectedConversation, setSelectedConversation] = useState<Conversation | null>(null);

    useEffect(() => {
        if (!selectedConversation) {
            setMessages(null);
            return;
        }
        let controller = new AbortController();
        (async () => {
            const messages = await getMessages(selectedConversation.id, token, controller.signal);
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
                const r = await sendMessage({
                    conversationId: selectedConversation.id,
                    text: message
                }, token);
                const messageResponse: Message = {
                    id: r.id,
                    from: r.from,
                    isMe: true,
                    text: r.text
                };
                setMessages((prev) => {
                    if (!prev) return [messageResponse];
                    return [...prev, messageResponse];
                });
                setMessage("");
            } catch (error) {
                if (error instanceof ApiError) {
                    toast.error(`${error.message}`);
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
        (async () => {
            try {
                const conversation = await getConversation(user.id, token);
                setConversations([conversation, ...conversations]);
                setSelectedConversation(conversation);
            } catch (error) {
                if (error instanceof ApiError) {
                    toast.error(`${error.message}`);
                }
            }
        })();
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
                    {conversations.map((conv) => (
                        <Card
                            key={conv.id}
                            className={`mb-2 cursor-pointer hover:bg-gray-100 transition duration-300 ${selectedConversation?.id === conv.id ? "bg-gray-200" : ""
                                }`}
                            onClick={() => setSelectedConversation(conv)}
                        >
                            <CardBody className="p-4">
                                <p className="font-semibold">{conv.with.name}</p>
                                {conv.lastMessage && <p className="text-sm text-gray-600">
                                    {conv.lastMessage.isMe ? "You: " : ""} {conv.lastMessage.text}
                                </p>}
                            </CardBody>
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
                                    key={msg.id}
                                    className={`flex ${msg.isMe ? "justify-end" : "justify-start"
                                        }`}
                                >
                                    <div
                                        className={`max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm ${msg.isMe
                                            ? "bg-blue-500 rounded-br-none"
                                            : "bg-gray-400 rounded-bl-none"
                                            }`}
                                    >
                                        {msg.text}
                                    </div>
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
        </div>
    );
}