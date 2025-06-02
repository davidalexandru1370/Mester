import { useEffect, useRef, useState } from "react";
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import NavMenu from "../NavMenu"
import useToken from '../useToken';
import { Button, Card, CardBody, CardHeader, Input, Label } from "reactstrap";
import { acceptTradesManJobResponse, ApiError, ClientJobRequest, Conversation, createTradesManJobResponse, getConversation, getConversations, getGlobalRequests, getMessages, MessageOrResponseOrBill, payBillRequest, sendBillRequest, sendMessage } from "@/api";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { Switch } from "../ui/switch";
import { useUser } from "@/context/UserContext";
import { useNavigate } from "react-router-dom";
import { Buffer } from "buffer"
import RequestDetails from "./RequestDetails";
type ConversationOrGlobalRequest = { conversation: Conversation, globalRequest: undefined } | { globalRequest: ClientJobRequest, conversation: undefined };

export default function () {
    const [conversations, setConversations] = useState<Conversation[]>([]);
    const [globalRequests, setGlobalRequests] = useState<ClientJobRequest[]>([]);
    const [combinedConversations, setCombinedConversations] = useState<ConversationOrGlobalRequest[]>([]);
    const [selectedConversation, setSelectedConversation] = useState<ConversationOrGlobalRequest | null>(null);
    const [isCurrentConversationApproved, setIsCurrentConversationApproved] = useState(false);

    const [messages, setMessages] = useState<MessageOrResponseOrBill[] | null>(null);


    const [dialogStateSendOffer, setDialogStateSendOffer] = useState<null | { conversationId: string }>(null);

    const [includeGlobalRequests, setIncludeGlobalRequests] = useState(false);
    const { token } = useToken();
    const { user } = useUser();
    if (!user) {
        const navigator = useNavigate();
        navigator("/auth")
    }
    const isMe = (message: MessageOrResponseOrBill) => {
        if (message.message) {
            return message.message.isMe;
        }
        // responses and bills are send only by tradesmen
        if (message.response || message.bill)
            return user?.isTradesman ?? false;

        throw new Error("Unknown message type");
    }
    const [date, setDate] = useState("")
    const [amount, setAmount] = useState("")
    const [createOfferDisabled, setCreateOfferDisabled] = useState(false)

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
        if (!messages) return;
        if (!selectedConversation) return;
        let responseApprovedId;
        if (selectedConversation.conversation) {
            responseApprovedId = selectedConversation.conversation.clientRequest.tradesManResponseApproveId
        } else {
            responseApprovedId = selectedConversation.globalRequest.tradesManResponseApproveId;
        }
        if (!responseApprovedId) {
            setIsCurrentConversationApproved(false);
            return;
        }
        for (const msg of messages) {
            if (msg.response && msg.response.id == responseApprovedId) {
                setIsCurrentConversationApproved(true);
                return;
            }
        }
        setIsCurrentConversationApproved(false);
    }, [messages]);

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
            console.log("Conversation selected:");
            console.log(selectedConversation);
            console.log(messages);
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

                const messageResponse: MessageOrResponseOrBill = {
                    sent: r.sent,
                    message: {
                        isMe: true,
                        from: r.from,
                        text: r.text,
                        id: r.id,
                    },
                    response: undefined,
                    bill: undefined
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


    const onOpenSendOffer = async (conversation: ConversationOrGlobalRequest) => {
        if (conversation.conversation) {
            setDialogStateSendOffer({ conversationId: conversation.conversation.id });
            return
        }
        setCreateOfferDisabled(true);
        try {
            const c = await getConversation(conversation.globalRequest.id, token);
            loadConversations();
            loadGlobalRequests();
            setDialogStateSendOffer({ conversationId: c.id });
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
        finally {
            setCreateOfferDisabled(false);
        };
    }


    const onSendOffer = async (conversationId: string, amount: number, endDate: string) => {
        try {
            const r = await createTradesManJobResponse({
                aproximationEndDate: endDate,
                workmanShipAmount: amount,
                conversationId: conversationId
            }, token);
            const newMessage: MessageOrResponseOrBill = {
                sent: r.sent,
                seen: r.seen,
                message: undefined,
                bill: undefined,
                response: {
                    id: r.id,
                    conversationId: r.conversationId,
                    aproximationEndDate: r.aproximationEndDate,
                    workmanshipAmount: r.workmanshipAmount,
                    sent: r.sent,
                    seen: r.seen,
                }
            }
            setMessages((prev) => {
                if (prev === null) {
                    return [newMessage]
                } else {
                    return [...prev, newMessage];
                }
            })
        }
        catch (error) {
            if (error instanceof ApiError) {
                if (error.error.type !== "aborted") {
                    console.log(error.message);
                    toast.error(`${error.message}`);
                }
                return;
            } else {
                throw error;
            }
        }
    };

    const onAcceptOffer = async (responseId: string) => {
        //TODO: update the UI to remove the accept response button and show that this response was accepted
        try {
            await acceptTradesManJobResponse(responseId, token);
            return true;
        } catch (error) {
            if (error instanceof ApiError) {
                if (error.error.type !== "aborted") {
                    console.log(error.message);
                    toast.error(`${error.message}`);
                }
                return false;
            } else {
                throw error;
            }
        }
    }


    const [billFormData, setBillFormData] = useState<{
        jobId: null | string,
        description: string,
        amount: string,
        image: null | string,
        error: null | string
    }>({
        jobId: null,
        description: "",
        amount: "",
        image: null,
        error: null
    });


    const scrollRef = useRef<HTMLDivElement>(null);
    useEffect(() => {
        if (scrollRef.current) {
            scrollRef.current.scrollIntoView({ behavior: "instant" });
        }
    }, [messages]);
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
                                    {user?.isTradesman === false && <p className="font-semibold">{conv.conversation.tradesMan.name}</p>}
                                    {user?.isTradesman === true && <p className="font-semibold">{conv.conversation.clientRequest.client.name}</p>}
                                    {conv.conversation.lastMessage && <p className="text-sm text-gray-600">
                                        {isMe(conv.conversation.lastMessage) ? "You: " : ""} {conv.conversation.lastMessage.message ? conv.conversation.lastMessage.message.text :
                                            conv.conversation.lastMessage.response ? `${conv.conversation.lastMessage.response.workmanshipAmount} by ${conv.conversation.lastMessage.response.aproximationEndDate}` :
                                                `Send bill of ${conv.conversation.lastMessage.bill.amount}`}
                                    </p>}
                                </CardBody>
                            </div>}
                            {conv.globalRequest && <div>
                                <CardHeader>{conv.globalRequest.title}</CardHeader>
                                <CardBody className="p-4">
                                    Global request by {conv.globalRequest.client.name}
                                </CardBody>
                            </div>}
                        </Card>
                    ))}
                </div>

                {/* Message Panel */}
                {selectedConversation && <div className="flex-1 flex flex-col overflow-y-auto max-h-screen p-4">
                    <div className="flex-1 overflow-y-auto p-6 bg-gradient-to-br from-white to-blue-50" >
                        <div className="gray-200">
                            <RequestDetails editable={false} disableUpdateButton={true} onUpdateRequestDetails={() => { }} initialRequestDetails={selectedConversation.conversation?.clientRequest ?? selectedConversation.globalRequest} />
                        </div>
                        <br />
                        <div className="space-y-4">
                            {messages ? "" : "Loading messages..."}
                            {messages?.map((msg, index) => (
                                <div
                                    key={msg.message ? msg.message.id : msg.response ? msg.response.id : msg.bill.id}
                                    className={`flex ${isMe(msg) ? "justify-end" : "justify-start"
                                        }`}
                                    ref={index === messages.length - 1 ? scrollRef : null}
                                >
                                    {msg.message ?
                                        //TODO: of coures I should create 3 different components for each type of message
                                        // message format
                                        <div
                                            className={`max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm ${isMe(msg)
                                                ? "bg-blue-500 rounded-br-none"
                                                : "bg-gray-400 rounded-bl-none"
                                                }`}
                                        >
                                            {msg.message.text}
                                        </div> :
                                        // response format
                                        msg.response ?
                                            <div
                                                className={`max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm ${!isMe(msg)
                                                    ?
                                                    "bg-gray-400 rounded-br-none" :
                                                    selectedConversation.conversation?.clientRequest.tradesManResponseApproveId === msg.response.id ?
                                                        "bg-green-500 rounded-br-none" :
                                                        "bg-blue-500 rounded-br-none"
                                                    }`}
                                            >
                                                <h4>Proposed resolution</h4>
                                                <p>Can be done by {msg.response.aproximationEndDate}</p>
                                                <p>The workmanship will be {msg.response.workmanshipAmount}</p>
                                                {selectedConversation.conversation?.clientRequest.tradesManResponseApproveId === msg.response.id && <div className="mt-2">Proposed accepted</div>}

                                                {!selectedConversation.conversation?.clientRequest.tradesManResponseApproveId && user?.isTradesman === false &&
                                                    <Button
                                                        className="mt-2"
                                                        onClick={async () => {
                                                            const r = await onAcceptOffer(msg.response.id);
                                                            if (r) {
                                                                toast.success("Job accepted!");
                                                            }
                                                        }}>Accept proposal</Button>
                                                }

                                                {selectedConversation.conversation?.clientRequest.tradesManResponseApproveId !== msg.response.id && user?.isTradesman === true &&
                                                    <div className="mt-2">Proposed send</div>
                                                }

                                            </div> :
                                            // bill format
                                            <div
                                                className={`max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm ${isMe(msg)
                                                    ? "bg-blue-500 rounded-br-none"
                                                    : "bg-gray-400 rounded-bl-none"
                                                    }`}
                                            >
                                                <h4>Bill request</h4>
                                                <p>{msg.bill.description}</p>
                                                <img src={msg.bill.billImage} alt="Bill" className="w-full h-auto rounded-lg mb-2" />
                                                {user?.isTradesman === true && (msg.bill.paid ? <p><b>The bill in value of {msg.bill.amount} was paid</b></p> : <p><b>The bill in value of {msg.bill.amount} is still not paid</b></p>)}
                                                {user?.isTradesman === false && (msg.bill.paid ?
                                                    <div><b>You paid {msg.bill.amount}</b></div> :
                                                    <div>
                                                        <p><b>You have to pay {msg.bill.amount}</b></p>
                                                        <Button
                                                            className="mt-2"
                                                            onClick={async () => {
                                                                try {
                                                                    await payBillRequest(msg.bill.id, token);
                                                                    msg.bill.paid = true;
                                                                    if (messages) {
                                                                        setMessages([...messages]);
                                                                    }
                                                                }
                                                                catch (error) {
                                                                    if (error instanceof ApiError) {
                                                                        if (error.error.type !== "aborted") {
                                                                            console.log(error);
                                                                            toast.error(`${error.message}`);
                                                                        }
                                                                    } else {
                                                                        throw error;
                                                                    }
                                                                }
                                                            }}>Mark as paid</Button>
                                                    </div>)}
                                            </div>
                                    }

                                </div>
                            ))}
                        </div>
                    </div>

                    <div className="p-4 border-t bg-white flex gap-2" ref={scrollRef}>
                        <Input
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            placeholder="Type a message..."
                            className="flex-1"
                            onKeyDown={(e) => e.key === "Enter" && onSendMessage()}
                        />
                        <Button onClick={onSendMessage}>Send</Button>
                        {user?.isTradesman && !selectedConversation.conversation?.clientRequest.jobApprovedId && !selectedConversation.globalRequest?.jobApprovedId &&
                            <Button variant="outline" onClick={() => onOpenSendOffer(selectedConversation)} disabled={createOfferDisabled}>
                                Create Offer
                            </Button>
                        }
                        {isCurrentConversationApproved && user?.isTradesman === true &&
                            <Button variant="outline" onClick={() => {
                                setBillFormData({
                                    jobId: selectedConversation.conversation?.clientRequest.jobApprovedId!,
                                    amount: "",
                                    description: "",
                                    image: null,
                                    error: null
                                })
                            }}>
                                Send bill
                            </Button>
                        }
                    </div>
                </div>
                }

                <Dialog open={!!billFormData.jobId} onOpenChange={(open) => {
                    if (!open) setBillFormData({
                        jobId: null,
                        amount: "",
                        description: "",
                        image: null,
                        error: null
                    });
                }}>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Send a bill</DialogTitle>
                        </DialogHeader>

                        <div>
                            <Label htmlFor="description">Description</Label>
                            <Input
                                type="text"
                                name="description"
                                value={billFormData.description}
                                onChange={(e) => {
                                    setBillFormData({
                                        ...billFormData,
                                        description: e.target.value,
                                    })
                                }}
                                placeholder="Enter description"
                            />
                        </div>

                        <div>
                            <Label htmlFor="amount">Amount</Label>
                            <Input
                                type="number"
                                name="amount"
                                value={billFormData.amount}
                                onChange={(e) => {
                                    setBillFormData({
                                        ...billFormData,
                                        amount: e.target.value,
                                    })
                                }}
                                placeholder="Enter amount"
                            />
                        </div>

                        <div>
                            <Label htmlFor="image">Image (required)</Label>
                            <Input
                                type="file"
                                name="image"
                                accept="image/jpeg, image/png"
                                onChange={e => {
                                    (async () => {
                                        if (e.target.files && e.target.files[0]) {
                                            const data = await e.target.files[0].arrayBuffer();
                                            setBillFormData({
                                                ...billFormData,
                                                image: Buffer.from(data).toString("base64"),
                                            });
                                            // setBillFormData({
                                            //     ...billFormData,
                                            //     image: await e.target.files[0].arrayBuffer()
                                            // })
                                        }
                                    })();
                                }}
                            />
                        </div>

                        {billFormData.error && <p className="text-red-500 text-sm">{billFormData.error}</p>}


                        <Button type="submit" onClick={async () => {
                            let amountNum;
                            try {
                                amountNum = parseFloat(billFormData.amount);
                            } catch (error) {
                                setBillFormData({ ...billFormData, error: "Invalid amount" });
                                return;
                            }
                            if (!billFormData.description.trim()) {
                                setBillFormData({ ...billFormData, error: "Description is required" })
                                return;
                            }
                            if (!billFormData.image) {
                                setBillFormData({ ...billFormData, error: "Image is required" })
                                return;
                            }
                            try {
                                const r = await sendBillRequest(billFormData.jobId!,
                                    {
                                        amount: amountNum,
                                        description: billFormData.description,
                                        billImageBase64: billFormData.image
                                    }, token);
                                setBillFormData({
                                    jobId: null,
                                    amount: "",
                                    description: "",
                                    image: null,
                                    error: null
                                });
                                const newMessage: MessageOrResponseOrBill = {
                                    sent: r.sent,
                                    seen: r.seen,
                                    message: undefined,
                                    bill: {
                                        id: r.id,
                                        amount: r.amount,
                                        description: r.description,
                                        paid: false, // TODO: handle paid state
                                        jobId: r.jobId,
                                        billImage: r.billImage,
                                        sent: r.sent,
                                        seen: r.seen
                                    },
                                    response: undefined
                                };
                                setMessages((prev) => {
                                    if (prev === null) {
                                        return [newMessage]
                                    } else {
                                        return [...prev, newMessage];
                                    }
                                });
                            } catch (error) {
                                if (error instanceof ApiError) {
                                    if (error.error.type !== "aborted") {
                                        console.log(error);
                                        toast.error(`${error.message}`);
                                    }
                                }
                            }
                        }}>Submit</Button>
                    </DialogContent>
                </Dialog>

                {/* SendRequest dialog */}
                <Dialog open={!!dialogStateSendOffer} onOpenChange={(open) => { if (!open) setDialogStateSendOffer(null); }}>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Create an offer</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Label htmlFor="date">Select an aproximated end date</Label>
                            <Input
                                id="date"
                                type="date"
                                value={date}
                                onChange={(e) => setDate(e.target.value)}
                            />
                        </div>

                        <div>
                            <Label htmlFor="amount">Workmapship amount</Label>
                            <Input
                                id="amount"
                                type="number"
                                step="0.01"
                                placeholder="0.00"
                                value={amount}
                                onChange={(e) => setAmount(e.target.value)}
                            />
                        </div>

                        <Button type="submit" onClick={async () => {
                            try {
                                const amountNum = parseFloat(amount);
                                if (date.trim() === "") throw new Error();
                                try {
                                    await onSendOffer(dialogStateSendOffer!.conversationId, amountNum, date);
                                    setDialogStateSendOffer(null);
                                } catch (error) {
                                    if (error instanceof ApiError) {
                                        if (error.error.type !== "aborted") {
                                            console.log(error);
                                            toast.error(`${error.message}`);
                                        }
                                    }
                                }
                            } catch (error) {
                                toast.error("Invalid input. Please check the amount and date.");
                            }
                        }}>Submit</Button>
                    </DialogContent>
                </Dialog>
            </div>

        </div >
    );
}