import { ApiError, clientGetRequestsConversations, ClientJobRequest, ClientJobRequestWithoutId, ClientJobResponsesConversation } from "@/api";
import { useEffect, useState } from "react";
import { Label } from "reactstrap";
import { Input } from "../ui/input";
import { Checkbox } from "../ui/checkbox";
import { Button } from "../ui/button";
import useToken from "../useToken";
import { toast } from "react-toastify";

interface RequestDetailsProps {
    initialRequestDetails?: ClientJobRequest,
    onUpdateRequestDetails: (details: ClientJobRequestWithoutId) => void;
    disableUpdateButton: boolean
}

export default function ({
    initialRequestDetails,
    onUpdateRequestDetails,
    disableUpdateButton
}: RequestDetailsProps) {
    const { token } = useToken();

    const initialValue: ClientJobRequestWithoutId = {
        id: initialRequestDetails?.id,
        description: initialRequestDetails?.description || "",
        title: initialRequestDetails?.title || "",
        startDate: initialRequestDetails?.startDate || "",
        imagesUrl: initialRequestDetails?.imagesUrl || [],
        requestedOn: initialRequestDetails?.requestedOn || "",
        showToEveryone: initialRequestDetails?.showToEveryone || false,
        open: initialRequestDetails?.open || true,
        jobApprovedId: initialRequestDetails?.jobApprovedId,
    };


    useEffect(() => {
        const initialValue: ClientJobRequestWithoutId = {
            id: initialRequestDetails?.id,
            description: initialRequestDetails?.description || "",
            title: initialRequestDetails?.title || "",
            startDate: initialRequestDetails?.startDate || "",
            imagesUrl: initialRequestDetails?.imagesUrl || [],
            requestedOn: initialRequestDetails?.requestedOn || "",
            showToEveryone: initialRequestDetails?.showToEveryone || false,
            open: initialRequestDetails?.open || true,
            jobApprovedId: initialRequestDetails?.jobApprovedId,
        };
        setSelectedRequest(initialValue);
    }, [initialRequestDetails]);
    const [selectedRequest, setSelectedRequest] = useState(initialValue);
    const [tradesManResponses, setTradesManResponses] = useState<ClientJobResponsesConversation[] | null>(null);
    useEffect(() => {
        if (!initialRequestDetails) return;
        let controller = new AbortController();
        (async () => {
            try {
                // TODO: fetch details page
                const messages = await clientGetRequestsConversations(initialRequestDetails.id, token, controller.signal);
                setTradesManResponses(messages);
            } catch (error) {
                if (error instanceof ApiError) {
                    toast.error(`${error.message}`);
                } else {
                    console.error("Unexpected error:", error);
                }
            }
        })();
        return () => {
            controller.abort();
        }
    }, [selectedRequest]);
    // const handleImageChange = (index: number, value: string) => {
    //     const updatedImages = [...formData.imagesUrl];
    //     updatedImages[index] = value;
    //     handleChange("imagesUrl", updatedImages);
    // };

    // const addImage = () => {
    //     handleChange("imagesUrl", [...formData.imagesUrl, ""]);
    // };

    // const removeImage = (index: number) => {
    //     const updatedImages = formData.imagesUrl.filter((_, i) => i !== index);
    //     handleChange("imagesUrl", updatedImages);
    // };
    const [includeStartDate, setIncludeStartDate] = useState(initialRequestDetails?.startDate ? true : false);

    return (
        <div className="max-w-xl">
            <h2>Edit Job Request</h2>

            <div className="space-y-4">
                {selectedRequest.requestedOn && <div>
                    <Label>Requested On {selectedRequest.requestedOn}</Label>
                </div>}

                <div>
                    <Checkbox checked={includeStartDate} onCheckedChange={e => {
                        if (e === "indeterminate") return;
                        setIncludeStartDate(e);
                    }}>Include Start Date</Checkbox>
                    {includeStartDate && <Label>Start Date</Label>}
                    {includeStartDate && <Input
                        type="date"
                        value={selectedRequest.startDate || ""}
                        onChange={(e) => {
                            setSelectedRequest({ ...selectedRequest, startDate: e.target.value })
                        }}
                    />}
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

                {/* <div className="space-y-2">
                        <Label>Images</Label>
                        {formData.imagesUrl.map((url, index) => (
                            <div key={index} className="flex items-center gap-2">
                                <Input
                                    value={url}
                                    onChange={(e) => handleImageChange(index, e.target.value)}
                                />
                                <Button variant="destructive" onClick={() => removeImage(index)}>
                                    Remove
                                </Button>
                            </div>
                        ))}
                        <Button variant="outline" onClick={addImage}>
                            Add Image
                        </Button>
                    </div> */}
                <div className="text-right">
                    <Button onClick={() => {
                        const r = { ...selectedRequest };
                        if (!includeStartDate) {
                            r.startDate = undefined;
                        }
                        onUpdateRequestDetails(r);
                    }} disabled={disableUpdateButton}>{initialRequestDetails ? "Update" : "Create"}</Button>
                </div>
            </div>


            <div className="space-y-4">
                {initialRequestDetails && !tradesManResponses && "Loading conversations..."}
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
    );
}


// <div className="space-y-4">
//     {tradesManResponses ? "" : "Loading conversations..."}
//     {tradesManResponses?.map((response) => (
//         <div
//             key={response.id}
//             className="flex justify-start"
//             onClick={() => {
//                 // TODO: navigate to the conversation with that user
//             }}
//         >
//             <p>{response.tradesMan.name}</p>
//             {response.response &&
//                 <div
//                     className={"max-w-xs px-4 py-2 rounded-2xl shadow-md text-white text-sm bg-gray-400 rounded-bl-none"}
//                 >
//                     <h4>Proposed resolution</h4>
//                     <p>Can be done by {response.response.AproximationEndDate}</p>
//                     <p>The workmanship will be {response.response.workmanshipAmount}</p>
//                 </div>
//             }
//         </div>
//     ))}
// </div >