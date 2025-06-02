import { ApiError, clientGetRequestsConversations, ClientJobRequest, ClientJobRequestCreateOrUpdate, ClientJobResponsesConversation } from "@/api";
import { useEffect, useState } from "react";
import { Label } from "reactstrap";
import { Input } from "../ui/input";
import { Checkbox } from "../ui/checkbox";
import { Button } from "../ui/button";
import useToken from "../useToken";
import { toast } from "react-toastify";
import { cn } from "@/lib/utils";
import { Buffer } from "buffer"
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from "../ui/carousel";
interface RequestDetailsProps {
    initialRequestDetails?: ClientJobRequest,
    onUpdateRequestDetails: (details: ClientJobRequestCreateOrUpdate) => void;
    disableUpdateButton: boolean
    editable: boolean;
}


type ClientJobRequestWithImagesUrls = ClientJobRequestCreateOrUpdate & { imagesUrl: string[] };

export default function ({
    initialRequestDetails,
    onUpdateRequestDetails,
    disableUpdateButton,
    editable
}: RequestDetailsProps) {
    const { token } = useToken();

    const initialValue: ClientJobRequestWithImagesUrls = {
        id: initialRequestDetails?.id,
        description: initialRequestDetails?.description || "",
        title: initialRequestDetails?.title || "",
        startDate: initialRequestDetails?.startDate || "",
        requestedOn: initialRequestDetails?.requestedOn || "",
        showToEveryone: initialRequestDetails?.showToEveryone || false,
        open: initialRequestDetails?.open || true,
        jobApprovedId: initialRequestDetails?.jobApprovedId,
        imagesUrl: initialRequestDetails?.imagesUrl ?? [],
        imagesBase64: []
    };


    useEffect(() => {
        const initialValue: ClientJobRequestWithImagesUrls = {
            id: initialRequestDetails?.id,
            description: initialRequestDetails?.description || "",
            title: initialRequestDetails?.title || "",
            startDate: initialRequestDetails?.startDate || "",
            requestedOn: initialRequestDetails?.requestedOn || "",
            showToEveryone: initialRequestDetails?.showToEveryone || false,
            open: initialRequestDetails?.open || true,
            jobApprovedId: initialRequestDetails?.jobApprovedId,
            imagesUrl: initialRequestDetails?.imagesUrl ?? [],
            imagesBase64: []
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
                    if (error.error.type !== "aborted") {
                        console.log(error);
                        toast.error(`${error.message}`);
                    }
                } else {
                    console.error("Unexpected error:", error);
                }
            }
        })();
        return () => {
            controller.abort();
        }
    }, [selectedRequest]);

    const [includeStartDate, _] = useState(initialRequestDetails?.startDate ? true : false);
    return (
        <div className="justify-center mt-6">
            <div className={cn("gap-4")}>
                {editable && initialRequestDetails && <h2>Edit Job Request</h2>}
                {editable && !initialRequestDetails && <h2>Create Job Request</h2>}
                <div className="space-y-4">
                    {selectedRequest.requestedOn && <div>
                        <Label>Requested On {selectedRequest.requestedOn}</Label>
                    </div>}

                    {/*
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
                </div>*/}

                    <div className="mx-25">
                        <Label>Title</Label>
                        <Input
                            value={selectedRequest.title}
                            onChange={(e) => setSelectedRequest({ ...selectedRequest, title: e.target.value })}
                            contentEditable={editable}
                        />
                    </div>

                    <div className="mx-25">
                        <Label>Description</Label>
                        <textarea className="flex justify-center items-center h-30 min-w-0 w-full border"
                            value={selectedRequest.description}
                            onChange={(e) => setSelectedRequest({ ...selectedRequest, description: e.target.value })}
                            contentEditable={editable}
                        />
                    </div>

                    {editable && <div>
                        <Checkbox contentEditable={editable}
                            checked={selectedRequest.showToEveryone}
                            onCheckedChange={(val) => {
                                if (val !== "indeterminate")
                                    setSelectedRequest({ ...selectedRequest, showToEveryone: val })
                            }
                            }

                        />
                        <Label className="ml-1">Show to Everyone</Label>
                    </div>}

                    {/* <div>
                        <Checkbox
                            checked={selectedRequest.open}
                            onCheckedChange={(val) => {
                                if (val !== "indeterminate")
                                    setSelectedRequest({ ...selectedRequest, open: val })
                            }}
                        />
                        <Label className="ml-1">Open</Label>
                    </div> */}

                    <div className="space-y-2">
                        {selectedRequest.imagesUrl.length > 0 &&
                            <div className="flex flex-col items-center justify-center">
                                <Carousel className="w-full max-w-xs">
                                    <CarouselContent>
                                        {selectedRequest.imagesUrl.map((image, index) => {
                                            return (
                                                <CarouselItem key={"carousel" + index}>
                                                    <img
                                                        src={image}
                                                    />
                                                </CarouselItem>
                                            );
                                        })}
                                    </CarouselContent>
                                    <CarouselPrevious />
                                    <CarouselNext />
                                </Carousel>
                            </div>}
                        {editable && <Input
                            type="file"
                            multiple
                            name="image"
                            accept="image/jpeg, image/png"
                            onChange={e => {
                                (async () => {
                                    if (e.target.files) {
                                        const imagesData: string[] = [];
                                        for (const file of e.target.files) {
                                            imagesData.push(Buffer.from(await file.arrayBuffer()).toString("base64"));
                                        }
                                        setSelectedRequest({ ...selectedRequest, imagesBase64: imagesData });
                                    }
                                })();
                            }}
                        />}
                    </div>
                    {editable && <div>
                        <Button onClick={() => {
                            const r: ClientJobRequestCreateOrUpdate = { ...selectedRequest };
                            if (!includeStartDate) {
                                r.startDate = undefined;
                            }
                            onUpdateRequestDetails(r);
                        }} disabled={disableUpdateButton}>{initialRequestDetails ? "Update" : "Create"}</Button>
                    </div>}
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
                                    <p>Can be done by {response.response.aproximationEndDate}</p>
                                    <p>The workmanship will be {response.response.workmanshipAmount}</p>
                                </div>
                            }
                        </div>
                    ))}
                </div>
            </div>
        </div >
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