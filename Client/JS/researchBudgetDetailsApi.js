// Client/JS/researchBudgetDetailsApi.js

const BASE_URL = "https://localhost:7097/api/ResearchBudgetDetails";

// מביא פרטי תקציב למחקר לפי researchId
export async function getResearchBudgetDetails(researchId) {
    const res = await fetch(`${BASE_URL}/${encodeURIComponent(researchId)}`);

    if (!res.ok) {
        let msg = "שגיאה בטעינת תקציב המחקר";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json();
}

console.log("researchBudgetDetailsApi.js loaded! BASE_URL =", BASE_URL);
