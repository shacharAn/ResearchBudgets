// Client/JS/usersApi.js

import { API_BASE } from "./config.js";
const BASE_URL = `${API_BASE}/users`;

//  REGISTER 
export async function registerUser(userData) {
    console.log("sending register:", userData);

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

//  LOGIN 
export async function login(userName, password) {
    console.log("sending login:", { userName, password });

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

//  GET USER WITH ROLES 
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

console.log("usersApi.js loaded! BASE_URL =", BASE_URL);
