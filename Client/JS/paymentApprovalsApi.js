// Client/JS/paymentApprovalsApi.js

const BASE_URL = "https://localhost:7097/api/paymentapprovals";

// יצירת אישור תשלום (עבור גורם מאשר)
export async function createPaymentApproval(approvalData) {
    const res = await fetch(BASE_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(approvalData)
    });

    if (!res.ok) {
        let msg = "שגיאה ביצירת אישור התשלום";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json(); // מחזיר את האובייקט PaymentApprovals
}

console.log("paymentApprovalsApi.js loaded! BASE_URL =", BASE_URL);
