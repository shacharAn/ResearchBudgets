// Client/JS/index.js
let budgetRequestsCache = [];

import { login, getUserWithRoles, registerUser } from "./usersApi.js";
import { getMyResearches } from "./researchesApi.js";
import { getResearchBudgetDetails } from "./researchBudgetDetailsApi.js";
import {
    createPaymentRequest,
    getPaymentRequestsByUser,
    getPaymentRequestsByResearch,
    deletePaymentRequest,
    updatePaymentRequest
} from "./paymentRequestsApi.js";
import { createPaymentApproval } from "./paymentApprovalsApi.js";
import { getAllBudgetCategories, createBudgetCategory } from "./budgetCategoriesApi.js";
import { uploadFileForResearch } from "./filesApi.js";

document.addEventListener("DOMContentLoaded", () => {
    //  התחברות 
    const loginForm = document.getElementById("login-form");
    if (loginForm) {
        loginForm.addEventListener("submit", onLoginSubmit);
    }

    //  הרשמה 
    const registerForm = document.getElementById("register-form");
    if (registerForm) {
        registerForm.addEventListener("submit", onRegisterSubmit);
    }

    //  דף המחקרים שלי 
    const researchesTable = document.getElementById("researches-table");
    if (researchesTable) {
        loadMyResearches();
    }

    //  דף תקציב מחקר 
    const budgetSummary = document.getElementById("budget-summary");
    if (budgetSummary) {
        loadBudgetDetails();
    }

    //  דף יצירת בקשת תשלום 
    const paymentRequestForm = document.getElementById("payment-request-form");
    if (paymentRequestForm) {
        paymentRequestForm.addEventListener("submit", onPaymentRequestSubmit);
        initNewPaymentRequestPage();
    }

    //  דף אישור תשלום 
    const approvalForm = document.getElementById("approval-form");
    if (approvalForm) {
        approvalForm.addEventListener("submit", onApprovalSubmit);
    }
    
    const myRequestsTable = document.getElementById("my-payment-requests-table");
    if (myRequestsTable) {
        loadMyPaymentRequests();
    }
});

//  פונקציות עזר 

function getCurrentUser() {
    const json = localStorage.getItem("currentUser");
    if (!json) return null;
    try {
        return JSON.parse(json);
    } catch {
        return null;
    }
}

function getUserIdNumber(user) {
    return user?.idNumber || user?.IdNumber || null;
}

//  LOGIN 

async function onLoginSubmit(e) {
    e.preventDefault();

    const userName = document.getElementById("login-username").value.trim();
    const password = document.getElementById("login-password").value.trim();
    const errorDiv = document.getElementById("login-error");

    if (errorDiv) errorDiv.textContent = "";

    if (!userName || !password) {
        if (errorDiv) errorDiv.textContent = "נא למלא שם משתמש וסיסמה.";
        return;
    }

    try {
        const user = await login(userName, password);

        let userWithRoles = null;
        try {
            userWithRoles = await getUserWithRoles(
                user.userName || user.UserName || userName
            );
        } catch {
            userWithRoles = { ...user, roles: [] };
        }

        localStorage.setItem("currentUser", JSON.stringify(userWithRoles));

        window.location.href = "my-researches.html";
    } catch (err) {
        console.error(err);
        if (errorDiv) {
            errorDiv.textContent = err.message || "שגיאה בהתחברות.";
        }
    }
}

//  REGISTER 

async function onRegisterSubmit(e) {
    e.preventDefault();

    const idNumber   = document.getElementById("reg-id").value.trim();
    const userName   = document.getElementById("reg-username").value.trim();
    const email      = document.getElementById("reg-email").value.trim();
    const firstName  = document.getElementById("reg-firstname").value.trim();
    const lastName   = document.getElementById("reg-lastname").value.trim();
    const password   = document.getElementById("reg-password").value;
    const confirmPwd = document.getElementById("reg-confirm").value;

    const errorDiv   = document.getElementById("register-error");
    const successDiv = document.getElementById("register-success");

    if (errorDiv) errorDiv.textContent = "";
    if (successDiv) successDiv.textContent = "";

    if (!idNumber || !userName || !email || !firstName || !lastName || !password || !confirmPwd) {
        if (errorDiv) errorDiv.textContent = "נא למלא את כל השדות.";
        return;
    }

    if (password !== confirmPwd) {
        if (errorDiv) errorDiv.textContent = "הסיסמה ואישור הסיסמה לא תואמים.";
        return;
    }

    try {
        await registerUser({
            idNumber,                    
            userName,                    
            email,                       
            password,                    
            confirmPassword: confirmPwd,
            firstName,                   
            lastName                     
        });

        if (successDiv) {
            successDiv.textContent = "ההרשמה הצליחה! אפשר כעת להתחבר.";
        }

        setTimeout(() => {
            window.location.href = "index.html";
        }, 2000);
    } catch (err) {
        console.error(err);
        if (errorDiv) {
            errorDiv.textContent = err.message || "שגיאה בהרשמה.";
        }
    }
}
//  עמוד בקשת תשלום חדשה 
async function initNewPaymentRequestPage() {
    const researchSelect = document.getElementById("pr-research-id");
    const categorySelect = document.getElementById("pr-category-id");
    const errorDiv = document.getElementById("pr-error");

    if (!researchSelect || !categorySelect) {
        console.warn("new-payment-request: missing selects");
        return;
    }

    if (errorDiv) errorDiv.textContent = "";

    const currentUserJson = localStorage.getItem("currentUser");
    if (!currentUserJson) {
        window.location.href = "index.html";
        return;
    }

    const currentUser = JSON.parse(currentUserJson);
    const idNumber = currentUser.idNumber || currentUser.IdNumber;

    if (!idNumber) {
        if (errorDiv) errorDiv.textContent = "לא נמצאה תעודת זהות של המשתמש.";
        return;
    }

    try {
        const researches = await getMyResearches(idNumber);
        console.log("initNewPaymentRequestPage - researches:", researches);

        researchSelect.innerHTML = `<option value="">בחר מחקר</option>`;

        researches.forEach(r => {
            const researchId   = r.researchId   ?? r.ResearchId;
            const researchCode = r.researchCode ?? r.ResearchCode ?? "";
            const researchName = r.researchName ?? r.ResearchName ?? "";

            const opt = document.createElement("option");
            opt.value = researchId;
            opt.textContent = `${researchCode} - ${researchName}`;
            researchSelect.appendChild(opt);
        });

        const params = new URLSearchParams(window.location.search);
        const fromUrl = params.get("researchId");
        if (fromUrl && researches.some(r => (r.researchId ?? r.ResearchId) == fromUrl)) {
            researchSelect.value = fromUrl;
        }

        await populateCategoriesForPaymentRequest();

        const newCatSection = document.getElementById("pr-new-category-section");
        const newCatNameInput = document.getElementById("pr-new-category-name");
        const newCatDescInput = document.getElementById("pr-new-category-desc");

        function toggleNewCategorySection() {
            if (!categorySelect) return;
            if (categorySelect.value === "__new__") {
                newCatSection.style.display = "block";
            } else {
                newCatSection.style.display = "none";
                if (newCatNameInput) newCatNameInput.value = "";
                if (newCatDescInput) newCatDescInput.value = "";
            }
        }

        toggleNewCategorySection();

        categorySelect.addEventListener("change", () => {
            toggleNewCategorySection();
        });
        researchSelect.addEventListener("change", async () => {
            await populateCategoriesForPaymentRequest();
        });


    } catch (err) {
        console.error("initNewPaymentRequestPage error:", err);
        if (errorDiv) {
            errorDiv.textContent = err.message || "שגיאה בטעינת רשימת המחקרים.";
        }
    }
}

//  המחקרים שלי 

async function loadMyResearches() {
    const currentUserJson = localStorage.getItem("currentUser");
    if (!currentUserJson) {
        window.location.href = "index.html";
        return;
    }

    const currentUser = JSON.parse(currentUserJson);

    const idNumber = currentUser.idNumber || currentUser.IdNumber;
    if (!idNumber) {
        console.error("לא נמצא IdNumber ב-currentUser:", currentUser);
        alert("שגיאה: לא נמצאה תעודת זהות של המשתמש.");
        return;
    }

    try {
        const researches = await getMyResearches(idNumber);

        const tableBody = document.getElementById("researches-table");
        tableBody.innerHTML = "";

        researches.forEach(r => {
            const researchId   = r.researchId   ?? r.ResearchId;
            const researchCode = r.researchCode ?? r.ResearchCode ?? "";
            const researchName = r.researchName ?? r.ResearchName ?? "";
            const totalBudget  = r.totalBudget  ?? r.TotalBudget  ?? "";
            const startDateRaw = r.startDate    ?? r.StartDate;
            const endDateRaw   = r.endDate      ?? r.EndDate;

            const startDate = startDateRaw ? startDateRaw.toString().substring(0, 10) : "";
            const endDate   = endDateRaw   ? endDateRaw.toString().substring(0, 10)   : "";

            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${researchCode}</td>
                <td>${researchName}</td>
                <td>${totalBudget}</td>
                <td>${startDate}</td>
                <td>${endDate}</td>
                <td>
                    <a href="research-budget.html?researchId=${researchId}" class="link">פירוט התקציב</a>
                    |
                    <a href="new-payment-request.html?researchId=${researchId}" class="link">בקשת תשלום</a>
                </td>
            `;

            tableBody.appendChild(tr);
        });
    } catch (err) {
        console.error(err);
        alert(err.message || "שגיאה בטעינת המחקרים.");
    }
}


//  תקציב מחקר 
async function loadBudgetDetails() {
    const currentUserJson = localStorage.getItem("currentUser");
    if (!currentUserJson) {
        window.location.href = "index.html";
        return;
    }

    const errorDiv = document.getElementById("budget-error");
    const titleEl  = document.getElementById("research-title");
    const summaryDiv = document.getElementById("budget-summary");
    const catBody  = document.getElementById("budget-categories-table");
    const reqBody  = document.getElementById("budget-requests-table");

    if (errorDiv) errorDiv.textContent = "";
    summaryDiv.innerHTML = "";
    catBody.innerHTML = "";
    reqBody.innerHTML = "";

    const params = new URLSearchParams(window.location.search);
    const researchIdStr = params.get("researchId");
    const researchId = researchIdStr ? parseInt(researchIdStr, 10) : NaN;

    if (!researchIdStr || isNaN(researchId) || researchId <= 0) {
        if (errorDiv) errorDiv.textContent = "מזהה מחקר (researchId) לא תקין בכתובת.";
        return;
    }

    try {
        const details = await getResearchBudgetDetails(researchId);

        if (titleEl) {
            titleEl.textContent = `${details.researchCode} - ${details.researchName}`;
        }

        const startDate = details.startDate ? details.startDate.toString().substring(0, 10) : "";
        const endDate   = details.endDate   ? details.endDate.toString().substring(0, 10)   : "";

        summaryDiv.innerHTML = `
            <p><strong>מספר מחקר:</strong> ${details.researchId}</p>
            <p><strong>שם מחקר:</strong> ${details.researchName}</p>
            <p><strong>קוד מחקר:</strong> ${details.researchCode}</p>
            <p><strong>תאריך התחלה:</strong> ${startDate}</p>
            <p><strong>תאריך סיום:</strong> ${endDate}</p>
            <p><strong>תקציב כולל:</strong> ${details.totalBudget}</p>
            <p><strong>סה"כ הוצאות מאושרות:</strong> ${details.totalApprovedExpenses}</p>
            <p><strong>יתרה:</strong> ${details.remainingBudget}</p>
        `;

        // הוצאות מאושרות
        const categories = details.expensesByCategory || details.ExpensesByCategory || [];
        categories.forEach(c => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${c.categoryName}</td>
                <td>${c.totalByCategory}</td>
            `;
            catBody.appendChild(tr);
        });

        reqBody.innerHTML = "";

        try {
            const requests = await getPaymentRequestsByResearch(researchId);
            budgetRequestsCache = requests; 
            console.log("budget requests for research", researchId, requests);

            requests.forEach(p => {
                const id =
                    p.paymentRequestId ?? p.PaymentRequestId;
                const amount =
                    p.amount ?? p.Amount;
                const category =
                    p.categoryName ?? p.CategoryName;
                const status =
                    p.status ?? p.Status;
                const desc =
                    p.description ?? p.Description ?? "";

                const dateRaw =
                    p.requestDate ?? p.RequestDate;
                const dateStr = dateRaw
                    ? dateRaw.toString().substring(0, 10)
                    : "";

                const requestedBy =
                    p.requestedBy ??
                    (
                        (p.requestedByFirstName ?? p.RequestedByFirstName ?? "") +
                        " " +
                        (p.requestedByLastName ?? p.RequestedByLastName ?? "")
                    ).trim();

                const fileRelativePath =
                    p.fileRelativePath ?? p.FileRelativePath ?? null;
                const fileOriginalName =
                    p.fileOriginalName ?? p.FileOriginalName ?? null;

                let fileHref = "";
                if (fileRelativePath) {
                    if (fileRelativePath.startsWith("http")) {
                        fileHref = fileRelativePath;
                    } else {
                        const cleanPath = fileRelativePath.replace(/^\/+/, "");
                        fileHref = `https://localhost:7097/${cleanPath}`;
                    }
                }

                const fileCellHtml = fileHref
                    ? `<a href="${fileHref}" target="_blank">${fileOriginalName || "צפה בקובץ"}</a>`
                    : "";

                const canEdit = status === "Pending";

                const tr = document.createElement("tr");
                tr.innerHTML = `
                    <td>${id}</td>
                    <td>${dateStr}</td>
                    <td>${amount}</td>
                    <td>${category}</td>
                    <td>${requestedBy}</td>
                    <td>${status}</td>
                    <td>${desc}</td>
                    <td>${fileCellHtml}</td>
                    <td>
                        ${
                            canEdit
                                ? `
                                <button class="btn-small btn-edit-request" data-id="${id}">עריכה</button>
                                <button class="btn-small btn-delete-request" data-id="${id}">מחיקה</button>
                                `
                                : ""
                        }
                    </td>
                `;
                reqBody.appendChild(tr);
            });

            reqBody.onclick = async (e) => {
                const btn = e.target.closest("button");
                if (!btn) return;

                const idAttr = btn.getAttribute("data-id");
                if (!idAttr) return;
                const id = parseInt(idAttr, 10);

                const p = budgetRequestsCache.find(
                    r => (r.paymentRequestId ?? r.PaymentRequestId) === id
                );
                if (!p) return;

                if (btn.classList.contains("btn-delete-request")) {
                    const ok = confirm("האם את בטוחה שברצונך למחוק את בקשת התשלום?");
                    if (!ok) return;

                    try {
                        await deletePaymentRequest(id);
                        alert("הבקשה נמחקה בהצלחה.");
                        await loadBudgetDetails(); 
                    } catch (err) {
                        console.error(err);
                        alert(err.message || "שגיאה במחיקת הבקשה.");
                    }
                }

                if (btn.classList.contains("btn-edit-request")) {
                    const currentAmount = p.amount ?? p.Amount;
                    const currentDesc   = p.description ?? p.Description ?? "";

                    const newAmountStr = prompt("סכום חדש:", currentAmount);
                    if (newAmountStr === null) return; 
                    const newAmount = parseFloat(newAmountStr);
                    if (!newAmount || newAmount <= 0) {
                        alert("סכום אינו תקין.");
                        return;
                    }

                    const newDesc = prompt("תיאור חדש:", currentDesc);
                    if (newDesc === null) return;

                    const payload = {
                        paymentRequestId: id,
                        researchId: p.researchId ?? p.ResearchId ?? researchId,
                        requestedById: p.requestedById ?? p.RequestedById,
                        categoryId: p.categoryId ?? p.CategoryId,
                        amount: newAmount,
                        description: newDesc || null,
                        fileId: p.fileId ?? p.FileId ?? null,
                        status: p.status ?? p.Status  
                    };

                    try {
                        await updatePaymentRequest(id, payload);
                        alert("הבקשה עודכנה בהצלחה.");
                        await loadBudgetDetails();
                    } catch (err) {
                        console.error(err);
                        alert(err.message || "שגיאה בעדכון הבקשה.");
                    }
                }
            };

        } catch (err2) {
            console.error("loadBudgetDetails - error loading requests:", err2);
            if (errorDiv) {
                errorDiv.textContent =
                    err2.message || "שגיאה בטעינת בקשות התשלום למחקר.";
            }
        }

    } catch (err) {
        console.error(err);
        if (errorDiv) {
            errorDiv.textContent = err.message || "שגיאה בטעינת פרטי התקציב.";
        }
    }
}

//  מילוי רשימת קטגוריות בדף בקשת תשלום 
async function populateCategoriesForPaymentRequest() {
    const categorySelect = document.getElementById("pr-category-id");
    const errorDiv = document.getElementById("pr-error");

    if (!categorySelect) {
        console.warn("populateCategoriesForPaymentRequest: לא נמצא pr-category-id בדף");
        return;
    }
    if (errorDiv) errorDiv.textContent = "";

    try {
        const categories = await getAllBudgetCategories();
        console.log("populateCategoriesForPaymentRequest - categories:", categories);

        categorySelect.innerHTML = `<option value="">בחר קטגוריה</option>`;

        categories.forEach(c => {
            const option = document.createElement("option");
            option.value = c.categoryId ?? c.CategoryId;
            option.textContent = c.categoryName ?? c.CategoryName;
            categorySelect.appendChild(option);
        });

        const newOpt = document.createElement("option");
        newOpt.value = "__new__";
        newOpt.textContent = "קטגוריה חדשה";
        categorySelect.appendChild(newOpt);

        if (categories.length === 0) {
            const option = document.createElement("option");
            option.value = "";
            option.disabled = true;
            option.textContent = "אין קטגוריות זמינות במערכת";
            categorySelect.appendChild(option);
        }

    } catch (err) {
        console.error("populateCategoriesForPaymentRequest error:", err);
        if (errorDiv) {
            errorDiv.textContent = err.message || "שגיאה בטעינת קטגוריות התקציב.";
        }
    }
}

//  יצירת בקשת תשלום 
async function onPaymentRequestSubmit(e) {
    e.preventDefault();

    const currentUser = getCurrentUser();
    if (!currentUser) {
        window.location.href = "index.html";
        return;
    }

    const researchIdStr = document.getElementById("pr-research-id").value.trim();
    const categoryIdStr = document.getElementById("pr-category-id").value.trim();
    const amountStr     = document.getElementById("pr-amount").value.trim();
    const requestDesc   = document.getElementById("pr-Description").value.trim();

    const newCatNameInput = document.getElementById("pr-new-category-name");
    const newCatDescInput = document.getElementById("pr-new-category-desc");

    const errorDiv    = document.getElementById("pr-error");
    const successDiv  = document.getElementById("pr-success");

    if (errorDiv) errorDiv.textContent = "";
    if (successDiv) successDiv.textContent = "";

    if (!researchIdStr || !categoryIdStr || !amountStr) {
        if (errorDiv) errorDiv.textContent = "נא למלא את כל השדות החובה.";
        return;
    }

    const researchId = parseInt(researchIdStr, 10);
    let categoryId   = null;
    const amount     = parseFloat(amountStr);
    const requestedById = getUserIdNumber(currentUser);

    if (!requestedById) {
        if (errorDiv) errorDiv.textContent = "שגיאה: לא נמצאה תעודת זהות של המשתמש.";
        return;
    }

    if (!researchId || researchId <= 0 || !amount || amount <= 0) {
        if (errorDiv) errorDiv.textContent = "ערכים מספריים אינם תקינים.";
        return;
    }

    try {
        if (categoryIdStr === "__new__") {
            const newName = newCatNameInput?.value.trim();
            const newDesc = newCatDescInput?.value.trim() || null;

            if (!newName) {
                if (errorDiv) errorDiv.textContent = "נא למלא שם לקטגוריה החדשה.";
                return;
            }

            const createdCat = await createBudgetCategory(newName, newDesc);
            categoryId = createdCat.categoryId ?? createdCat.CategoryId;

            if (!categoryId) {
                if (errorDiv) errorDiv.textContent = "שגיאה ביצירת הקטגוריה החדשה.";
                return;
            }
        } else {
            categoryId = parseInt(categoryIdStr, 10);
        }

        if (!categoryId || categoryId <= 0) {
            if (errorDiv) errorDiv.textContent = "קטגוריה אינה תקינה.";
            return;
        }

        const fileInput = document.getElementById("pr-file");
        const file = fileInput && fileInput.files && fileInput.files.length > 0
            ? fileInput.files[0]
            : null;

        let fileId = null;

        if (file) {
            const fileResult = await uploadFileForResearch(researchId, requestedById, file);
            fileId = fileResult.fileId || fileResult.FileId || null;
        }

        const payload = {
            researchId,
            requestedById,
            categoryId,
            amount,
            description: requestDesc || null,
            fileId       
        };

        const result = await createPaymentRequest(payload);

        if (successDiv) {
            successDiv.textContent =
                `בקשת התשלום נוצרה בהצלחה. מספר בקשה: ${result.paymentRequestId || result.PaymentRequestId || ""}`;
        }

        (e.target).reset();

        await populateCategoriesForPaymentRequest();

    } catch (err) {
        console.error(err);
        if (errorDiv) {
            errorDiv.textContent = err.message || "שגיאה ביצירת בקשת התשלום.";
        }
    }
}


//  יצירת אישור תשלום 

async function onApprovalSubmit(e) {
    e.preventDefault();

    const currentUser = getCurrentUser();
    if (!currentUser) {
        window.location.href = "index.html";
        return;
    }

    const paymentRequestIdStr = document.getElementById("pa-request-id").value.trim();
    const approvalRole        = document.getElementById("pa-role").value.trim();
    const status              = document.getElementById("pa-status").value.trim();
    const comment             = document.getElementById("pa-comment").value.trim();
    const errorDiv            = document.getElementById("pa-error");
    const successDiv          = document.getElementById("pa-success");

    if (errorDiv) errorDiv.textContent = "";
    if (successDiv) successDiv.textContent = "";

    if (!paymentRequestIdStr || !approvalRole || !status) {
        if (errorDiv) errorDiv.textContent = "נא למלא את כל שדות החובה.";
        return;
    }

    const paymentRequestId = parseInt(paymentRequestIdStr, 10);
    const approvedById = getUserIdNumber(currentUser);

    if (!approvedById) {
        if (errorDiv) errorDiv.textContent = "שגיאה: לא נמצאה תעודת זהות של המשתמש.";
        return;
    }

    if (!paymentRequestId || paymentRequestId <= 0) {
        if (errorDiv) errorDiv.textContent = "מספר בקשת התשלום אינו חוקי.";
        return;
    }

    try {
        const payload = {
            paymentRequestId,
            approvedById,
            approvalRole,
            status,
            comment: comment || null
        };

        await createPaymentApproval(payload);

        if (successDiv) {
            successDiv.textContent = "אישור התשלום נשמר בהצלחה.";
        }

        (e.target).reset();
    } catch (err) {
        console.error(err);
        if (errorDiv) {
            errorDiv.textContent = err.message || "שגיאה ביצירת אישור תשלום.";
        }
    }
}
let myPaymentRequestsCache = [];
async function loadMyPaymentRequests() {
    const currentUserJson = localStorage.getItem("currentUser");
    if (!currentUserJson) {
        window.location.href = "index.html";
        return;
    }

    const currentUser = JSON.parse(currentUserJson);
    const requestedById = currentUser.idNumber || currentUser.IdNumber;

    if (!requestedById) {
        alert("שגיאה: לא נמצאה תעודת זהות של המשתמש.");
        return;
    }

    try {
        const list = await getPaymentRequestsByUser(requestedById);
        myPaymentRequestsCache = list; // נשמור בזיכרון

        const tbody = document.getElementById("my-payment-requests-table");
        tbody.innerHTML = "";

        list.forEach(p => {
            const id         = p.paymentRequestId ?? p.PaymentRequestId;
            const research   = p.researchName    ?? p.ResearchName;
            const category   = p.categoryName    ?? p.CategoryName;
            const amount     = p.amount          ?? p.Amount;
            const status     = p.status          ?? p.Status;
            const dateRaw    = p.requestDate     ?? p.RequestDate;
            const dateStr    = dateRaw ? dateRaw.toString().substring(0, 10) : "";

            const canEdit = status === "Pending";

            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${id}</td>
                <td>${research}</td>
                <td>${category}</td>
                <td>${amount}</td>
                <td>${dateStr}</td>
                <td>${status}</td>
                <td>
                    ${
                        canEdit
                            ? `
                            <button class="btn-small btn-edit" data-id="${id}">עריכה</button>
                            <button class="btn-small btn-delete" data-id="${id}">מחיקה</button>
                            `
                            : ""
                    }
                </td>
            `;
            tbody.appendChild(tr);
        });

        tbody.onclick = async (e) => {
            const btn = e.target;
            if (!(btn instanceof HTMLElement)) return;

            const idAttr = btn.getAttribute("data-id");
            if (!idAttr) return;
            const id = parseInt(idAttr, 10);

            const p = myPaymentRequestsCache.find(
                r => (r.paymentRequestId ?? r.PaymentRequestId) === id
            );
            if (!p) return;

            if (btn.classList.contains("btn-delete")) {
                const ok = confirm("האם את בטוחה שברצונך למחוק את בקשת התשלום?");
                if (!ok) return;

                try {
                    await deletePaymentRequest(id);
                    alert("הבקשה נמחקה בהצלחה.");
                    await loadMyPaymentRequests(); 
                } catch (err) {
                    console.error(err);
                    alert(err.message || "שגיאה במחיקת הבקשה.");
                }
            }

            if (btn.classList.contains("btn-edit")) {
                const currentAmount = p.amount ?? p.Amount;
                const currentDesc   = p.description ?? p.Description ?? "";

                const newAmountStr = prompt("סכום חדש:", currentAmount);
                if (newAmountStr === null) return; 
                const newAmount = parseFloat(newAmountStr);
                if (!newAmount || newAmount <= 0) {
                    alert("סכום אינו תקין.");
                    return;
                }

                const newDesc = prompt("תיאור חדש:", currentDesc);
                if (newDesc === null) return; 

                const payload = {
                    paymentRequestId: id,
                    researchId: p.researchId ?? p.ResearchId,
                    requestedById: p.requestedById ?? p.RequestedById ?? requestedById,
                    categoryId: p.categoryId ?? p.CategoryId,
                    amount: newAmount,
                    description: newDesc || null,
                    fileId: p.fileId ?? p.FileId ?? null,
                    status: p.status ?? p.Status   
                };

                try {
                    await updatePaymentRequest(id, payload);
                    alert("הבקשה עודכנה בהצלחה.");
                    await loadMyPaymentRequests(); 
                } catch (err) {
                    console.error(err);
                    alert(err.message || "שגיאה בעדכון הבקשה.");
                }
            }
        };

    } catch (err) {
        console.error(err);
        alert(err.message || "שגיאה בטעינת בקשות התשלום.");
    }
}