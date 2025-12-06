// Client/JS/usersApi.js

import { API_BASE } from "./config.js";
const BASE_URL = `${API_BASE}/users`;

export async function registerUser(userData) {
    const res = await fetch(`${BASE_URL}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(userData)
    });

    if (!res.ok) {
        let errMsg = "Registration failed";
        try {
            const err = await res.json();
            if (err && err.message) errMsg = err.message;
        } catch { }
        throw new Error(errMsg);
    }

    return res.json();
}

export async function login(userName, password) {
    const res = await fetch(`${BASE_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userName, password })
    });

    if (!res.ok) {
        let errMsg = "Login failed";
        try {
            const err = await res.json();
            if (err && err.message) errMsg = err.message;
        } catch { }
        throw new Error(errMsg);
    }

    return res.json();
}

export async function getUserWithRoles(userName) {
    const res = await fetch(`${BASE_URL}/${encodeURIComponent(userName)}/roles`);

    if (!res.ok) {
        let errMsg = "Failed to load roles";
        try {
            const err = await res.json();
            if (err && err.message) errMsg = err.message;
        } catch { }
        throw new Error(errMsg);
    }

    return res.json();
}

