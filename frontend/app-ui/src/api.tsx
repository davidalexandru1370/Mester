import axios from "axios";

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

/**
 * @throws OnlyOtherError
 */
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
    isMe: boolean;
    text: string
}

export interface Conversation {
    id: string;
    with: ConversationUser;
    lastMessage?: Message;
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
                }
            );
        return response.data as Conversation[];
    }
    catch (e) {
        throw new ApiError(convertError(e))
    }
}

export async function getMessages(conversationId: string, token: string, signal?: AbortSignal): Promise<Message[]> {
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
        return response.data as Message[];
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

export async function getConversation(withUserId: string, token: string, signal?: AbortSignal): Promise<Conversation> {
    try {
        const response = await axios
            .put(
                `${BASE_URL}/api/Conversations/users/${withUserId}`,
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
    sent: any;
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