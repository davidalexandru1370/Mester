import axios from "axios";
import { Interface } from "readline";

const BASE_URL = "https://localhost:8081";


export class ApiError extends Error {
    error: NormalErrors
    constructor(error: NormalErrors) {
        super()
        this.name = "ApiError"
        this.error = error
    }
}

interface AuthenticationError {
    type: "authenticationError"
}

interface OtherError {
    type: "other",
    message: string
}

interface AbortedError {
    type: "aborted"
}

type NormalErrors = OtherError | AuthenticationError | AbortedError

function convertError(e: unknown): NormalErrors {
    if (axios.isCancel(e)) {
        return {
            type: "aborted"
        }
    }
    if (axios.isAxiosError(e)) {
        if (!e.response) {
            return {
                type: "other",
                message: "Error reaching the server"
            }
        }
        if (e.response.status === 401) {
            return {
                type: "authenticationError"
            }
        }
        if (e.response.status === 500 && typeof e.response.data.message === "string") {
            return {
                type: "other",
                message: e.response.data.message
            }
        }
        return {
            type: "other",
            message: e.message
        }
    }
    if (e instanceof Error) {
        return {
            type: "other",
            message: e.message
        }
    }
    throw e
}

interface LoginResponse {
    expiresIn: number,
    jwt: string
}

export async function loginUser(email: string, password: string, phoneNumber: string, signal?: AbortSignal): Promise<LoginResponse> {
    try {
        const response = await axios.post(
            `${BASE_URL}/api/user/login`,
            {
                email: email,
                password: password,
                phoneNumber: phoneNumber
            },
            {
                signal
            }
        );
        return response.data as LoginResponse
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}

export async function registerUser(email: string, password: string, phoneNumber: string, signal?: AbortSignal): Promise<undefined> {
    try {
        const response = await axios
            .post(
                `${BASE_URL}/api/user/createAccount`,
                {
                    email: email,
                    password: password,
                    phoneNumber: phoneNumber,
                },
                {
                    timeout: 5000,
                    headers: {
                        "Content-Type": "application/json",
                        accept: "application/json", // If you receieve JSON response.
                    },
                    signal
                }
            );
        console.log(response.data.success);
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}


export interface ConversationUser {
    id: string;
    name: string;
    imageUrl?: string;
}

export interface Message {
    id: string;
    from: ConversationUser;
    text: string
}

export interface ClientJobRequestWithoutId {
    id?: string;
    requestedOn: string;
    startDate?: string;
    title: string;
    description: string;
    showToEveryone: boolean;
    open: boolean;
    imagesUrl: string[];
    jobApprovedId?: string;
}

export interface ClientJobRequest {
    id: string;
    requestedOn: string;
    startDate?: string;
    title: string;
    description: string;
    showToEveryone: boolean;
    open: boolean;
    imagesUrl: string[];
    jobApprovedId?: string;
}

interface MessageOrResponseApi {
    isMe: boolean;
    message?: Message;
    response?: TradesManJobResponse;
}

export type MessageOrResponse = MessageOnly | ResponseOnly;

export interface MessageOnly extends Message {
    type: "message";
    isMe: boolean;
}

export interface ResponseOnly extends TradesManJobResponse {
    type: "response";
    isMe: boolean;
}

export interface Conversation {
    id: string;
    clientRequest: ClientJobRequest;
    tradesMan: ConversationUser;
    lastMessage: MessageOrResponse;
}

export async function getConversations(token: string, signal?: AbortSignal): Promise<Conversation[]> {
    try {
        const response = await axios
            .get(
                `${BASE_URL}/api/Conversations`,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                }
            );
        return response.data as Conversation[];
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}

function convertMessageOrResponse(data: MessageOrResponseApi): MessageOrResponse {
    if (data.message) {
        return {
            type: "message",
            isMe: data.isMe,
            ...data.message
        };
    }
    if (data.response) {
        return {
            type: "response",
            isMe: data.isMe,
            ...data.response
        };
    }
    throw new Error("Invalid data format");
}

export async function getMessages(conversationId: string, token: string, signal?: AbortSignal): Promise<MessageOrResponse[]> {
    try {
        const response = await axios
            .get(
                `${BASE_URL}/api/Conversations/${conversationId}`,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        let data = response.data as MessageOrResponseApi[];
        return data.map(convertMessageOrResponse);
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}


export interface FindTradesMan {
    id: string;
    name: string;
    imageUrl: string;
}

export async function findTradesMan(params: { pattern: string, limit?: number }, token: string, signal?: AbortSignal): Promise<FindTradesMan[]> {
    let { pattern, limit } = params;
    limit ??= 10;
    try {
        const response = await axios
            .get(
                `${BASE_URL}/api/User/tradesmen?pattern=${pattern}&limit=${limit}`,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        return response.data as FindTradesMan[];
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}

export async function sendClientRequestToConversation(clientRequest: string, tradesManId: string, token: string, signal?: AbortSignal): Promise<undefined> {
    try {
        await axios
            .post(
                `${BASE_URL}/api/Requests/${clientRequest}/send/tradesmen/${tradesManId}`,
                undefined,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }

}

export async function getConversation(props: {
    clientJobRequestId: string,
    tradesManId: string
},
    token: string, signal?: AbortSignal): Promise<Conversation> {
    const { clientJobRequestId, tradesManId } = props;
    try {
        const response = await axios
            .put(
                `${BASE_URL}/api/Conversations/jobRequests/${clientJobRequestId}/tradesmen/${tradesManId}`,
                undefined,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        return response.data as Conversation;
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}

export interface SendMessageResponse {
    id: string;
    text: string;
    sent: string;
    from: ConversationUser;
}

export async function sendMessage(params: {
    conversationId: string,
    text: string,
}, token: string, signal?: AbortSignal): Promise<SendMessageResponse> {
    const { conversationId, text } = params;
    try {
        const response = await axios
            .post(
                `${BASE_URL}/api/Conversations/${conversationId}/Send`,
                {
                    text: text
                },
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        return response.data as SendMessageResponse;
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}


export async function clientGetRequests(token: string, signal?: AbortSignal): Promise<ClientJobRequest[]> {
    try {
        const response = await axios
            .get(
                `${BASE_URL}/api/Requests`,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        return response.data as ClientJobRequest[];
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}


export interface ClientJobResponsesConversation {
    id: string;
    tradesMan: ConversationUser;
    response?: TradesManJobResponse;
}

export async function clientGetRequestsConversations(requestId: string, token: string, signal?: AbortSignal): Promise<ClientJobResponsesConversation[]> {
    try {
        const response = await axios
            .get(
                `${BASE_URL}/api/Requests/${requestId}/conversations`,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        return response.data as ClientJobResponsesConversation[];
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}

interface TradesManJobResponse {
    id: string;
    clientJobRequestId: string;
    workmanshipAmount: number;
    AproximationEndDate: string;
}


export async function createRequest(params: ClientJobRequestWithoutId, token: string, signal?: AbortSignal): Promise<ClientJobRequest> {
    try {
        const response = await axios
            .post(
                `${BASE_URL}/api/Requests`,
                params,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        return response.data as ClientJobRequest;
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}

export async function updateRequest(params: ClientJobRequest, token: string, signal?: AbortSignal): Promise<ClientJobRequest> {
    try {
        const response = await axios
            .patch(
                `${BASE_URL}/api/Requests/${params.id}`,
                params,
                {
                    timeout: 5000,
                    headers: {
                        Authorization: `Bearer ${token}`
                    },
                    signal
                },
            );
        return response.data as ClientJobRequest;
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}