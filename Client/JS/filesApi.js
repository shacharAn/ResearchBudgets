import { API_BASE } from "./config.js";

export async function uploadFileForResearch(researchId, uploadedById, file) {
    const formData = new FormData();
    formData.append("ResearchId", researchId);
    formData.append("UploadedById", uploadedById);
    formData.append("File", file);  

    const res = await fetch(`${API_BASE}/Files/upload`, {
        method: "POST",
        body: formData
    });

    if (!res.ok) {
        let msg = "שגיאה בהעלאת הקובץ";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    const data = await res.json();
    return data;
}
