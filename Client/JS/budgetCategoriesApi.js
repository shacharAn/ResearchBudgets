// Client/JS/budgetCategoriesApi.js
import { API_BASE } from "./config.js";
const BASE_URL = `${API_BASE}/budgetcategories`;

export async function getAllBudgetCategories() {
    const res = await fetch(BASE_URL);

    if (!res.ok) {
        let msg = "שגיאה בטעינת קטגוריות התקציב";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json(); // מחזיר מערך של BudgetCategories
}
export async function createBudgetCategory(categoryName, description) {
    const res = await fetch(BASE_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            categoryName,
            description
        })
    });

    if (!res.ok) {
        let msg = "שגיאה ביצירת קטגוריה חדשה";
        try {
            const err = await res.json();
            if (err.message) msg = err.message;
        } catch {}
        throw new Error(msg);
    }

    return res.json(); // מחזיר את האובייקט BudgetCategories עם CategoryId חדש
}

