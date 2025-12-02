// Client/JS/researchesApi.js

const BASE_URL = "https://localhost:7097/api/researches";

// מביא את כל המחקרים לפי תעודת זהות 
export async function getMyResearches(idNumber) {
    const res = await fetch(`${BASE_URL}/by-user/${encodeURIComponent(idNumber)}`);

    if (!res.ok) {
        let msg = "Failed to load researches";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json();
}

console.log("researchesApi.js loaded! BASE_URL =", BASE_URL);
