// Client/JS/paymentRequestsApi.js
import { API_BASE } from "./config.js";
const BASE_URL = `${API_BASE}/paymentrequests`;

// יצירת בקשת תשלום חדשה
export async function createPaymentRequest(requestData) {
    const res = await fetch(BASE_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(requestData)
    });

    if (!res.ok) {
        let msg = "שגיאה ביצירת בקשת תשלום";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json(); // מחזיר { paymentRequestId: ... }
}

// (לא חובה, אבל שימושי) – כל הבקשות לפי מחקר
export async function getPaymentRequestsByResearch(researchId) {
    const res = await fetch(`${BASE_URL}/by-research/${encodeURIComponent(researchId)}`);

    if (!res.ok) {
        let msg = "שגיאה בטעינת בקשות התשלום למחקר";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json();
}

// (לא חובה, אבל שימושי) – כל הבקשות של משתמש
export async function getPaymentRequestsByUser(requestedById) {
    const res = await fetch(`${BASE_URL}/by-user/${encodeURIComponent(requestedById)}`);

    if (!res.ok) {
        let msg = "שגיאה בטעינת בקשות התשלום של המשתמש";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json();
}
// מחיקת בקשת תשלום
export async function deletePaymentRequest(id) {
    const res = await fetch(`${BASE_URL}/${id}`, {
        method: "DELETE"
    });

    if (!res.ok) {
        let msg = "שגיאה במחיקת בקשת התשלום";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }
}
// עדכון בקשת תשלום (רק כש-Pending)
export async function updatePaymentRequest(id, data) {
    const res = await fetch(`${BASE_URL}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        let msg = "שגיאה בעדכון בקשת התשלום";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }
}

console.log("paymentRequestsApi.js loaded! BASE_URL =", BASE_URL);
