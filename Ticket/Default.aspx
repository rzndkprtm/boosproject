<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Ticket_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        /* ========================================
           GLOBAL
           ======================================== */

        .ticket-app, .ticket-app * { box-sizing: border-box; }

        .ticket-app {
            height: calc(100vh - 150px);
            min-height: 600px;
            display: flex;
            background: #ffffff;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            overflow: hidden;
            font-family: 'Segoe UI', sans-serif;
        }

        /* ========================================
           SIDEBAR
           ======================================== */

        .ticket-app .sidebar {
            width: 380px;
            min-width: 380px;
            display: flex;
            flex-direction: column;
            background: #ffffff;
            border-right: 1px solid #e5e7eb;
            min-height: 0;
        }

        .ticket-app .sidebar-header {
            padding: 20px;
            border-bottom: 1px solid #eef2f7;
            flex-shrink: 0;
        }

        .ticket-app .sidebar-header h4 {
            margin: 0 0 15px;
            font-size: 20px;
            font-weight: 700;
            color: #1f2937;
        }

        .ticket-app .btn-new-ticket {
            width: 100%;
        }

        .ticket-app .sidebar-search {
            padding: 15px;
            border-bottom: 1px solid #eef2f7;
            flex-shrink: 0;
        }

        .ticket-app .ticket-list {
            flex: 1;
            min-height: 0;
            overflow-y: auto;
            overflow-x: hidden;
        }

        .ticket-app .ticket-item {
            padding: 16px 18px;
            border-bottom: 1px solid #f1f3f5;
            cursor: pointer;
            transition: background-color 0.2s ease;
        }

        .ticket-app .ticket-item:hover {
            background: #f8fafc;
        }

        .ticket-app .ticket-item.active {
            background: #eff6ff;
            border-left: 4px solid #2563eb;
            padding-left: 14px;
        }

        .ticket-app .ticket-top {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 10px;
            margin-bottom: 8px;
        }

        .ticket-app .ticket-id {
            font-size: 14px;
            font-weight: 700;
            color: #1f2937;
            overflow-wrap: anywhere;
        }

        .ticket-app .ticket-time {
            font-size: 12px;
            color: #9ca3af;
            white-space: nowrap;
            flex-shrink: 0;
        }

        .ticket-app .ticket-subject {
            margin-bottom: 10px;
            font-size: 14px;
            line-height: 1.5;
            color: #4b5563;
            overflow-wrap: anywhere;
        }

        .ticket-app .ticket-bottom {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 10px;
        }

        .ticket-app .unread {
            min-width: 22px;
            height: 22px;
            padding: 0 6px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border-radius: 50px;
            background: #dc3545;
            color: #ffffff;
            font-size: 12px;
            flex-shrink: 0;
        }

        /* ========================================
           CHAT PANEL
           ======================================== */

        .ticket-app .chat-panel {
            flex: 1;
            min-width: 0;
            min-height: 0;
            display: flex;
            flex-direction: column;
            overflow: hidden;
            background: #ffffff;
        }

        /* ========================================
           CHAT HEADER
           ======================================== */

        .ticket-app .chat-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 15px;
            padding: 18px 24px;
            background: #ffffff;
            border-bottom: 1px solid #eef2f7;
            flex-shrink: 0;
        }

        .ticket-app .customer-info {
            min-width: 0;
        }

        .ticket-app .customer-info h5 {
            margin: 0;
            font-size: 17px;
            font-weight: 700;
            color: #1f2937;
            overflow-wrap: anywhere;
        }

        .ticket-app .customer-info small {
            display: block;
            margin-top: 4px;
            font-size: 12px;
            color: #7b8494;
        }

        .ticket-app .header-actions {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            gap: 10px;
            flex-shrink: 0;
        }

        /* ========================================
           CHAT BODY
           ======================================== */

        .ticket-app .chat-body {
            flex: 1 1 0;
            min-height: 0;
            min-width: 0;

            display: block;

            overflow-y: auto;
            overflow-x: hidden;

            padding: 24px;
            background: #f8fafc;
        }

        /* ========================================
           CHAT DATE
           ======================================== */

        .ticket-app .chat-date {
            display: block;
            width: 100%;
            margin: 0 0 24px;

            text-align: center;
            font-size: 12px;
            line-height: 18px;
            color: #8b95a5;
        }

        /* ========================================
           MESSAGE ROW
           ======================================== */

        .ticket-app .message {
            display: flex;
            align-items: flex-start;

            width: 100%;
            height: auto;
            min-height: 0;

            margin: 0 0 20px;
            padding: 0;
        }

        /* Received berada di kiri */
        .ticket-app .message.received {
            justify-content: flex-start;
        }

        /* Sent berada di kanan */
        .ticket-app .message.sent {
            justify-content: flex-end;
        }

        /* ========================================
           MESSAGE CONTENT
           ======================================== */

        .ticket-app .message-content {
            display: block;

            width: auto;
            max-width: 70%;
            min-width: 0;

            height: auto;
            min-height: 0;

            flex: 0 1 auto;
            align-self: flex-start;
        }

        /* ========================================
           MESSAGE NAME
           ======================================== */

        .ticket-app .message-name {
            display: block;

            width: 100%;
            height: auto;
            min-height: 0;

            margin: 0 0 5px;
            padding: 0;

            font-size: 12px;
            font-weight: 500;
            line-height: 16px;

            color: #7b8494;

            overflow-wrap: anywhere;
        }

        /* ========================================
           MESSAGE BUBBLE
           ======================================== */

        .ticket-app .bubble {
            /*
               Ukuran mengikuti isi teks.
               Tidak ada tinggi tetap.
            */

            display: table;

            width: auto;
            max-width: 100%;
            min-width: 0;

            height: auto;
            min-height: 0;

            margin: 0;
            padding: 8px 12px;

            border: 1px solid transparent;
            border-radius: 12px;

            font-family: 'Segoe UI', sans-serif;
            font-size: 14px;
            font-weight: 400;
            line-height: 20px;

            /*
               Teks selalu rata kiri.
            */

            text-align: left;

            /*
               Baris baru tetap dipertahankan.
               Teks panjang otomatis turun baris.
            */

            white-space: pre-wrap;
            overflow-wrap: anywhere;
            word-break: normal;

            vertical-align: top;
        }

        /* ========================================
           RECEIVED BUBBLE
           ======================================== */

        .ticket-app .message.received .bubble {
            background: #ffffff;
            color: #273142;

            border-color: #e5e7eb;
            border-top-left-radius: 4px;
        }

        /* ========================================
           SENT BUBBLE
           ======================================== */

        .ticket-app .message.sent .bubble {
            background: #2563eb;
            color: #ffffff;

            border-color: #2563eb;
            border-top-right-radius: 4px;
        }

        /* ========================================
           MESSAGE TIME
           ======================================== */

        .ticket-app .message-time {
            display: block;

            width: 100%;
            height: auto;
            min-height: 0;

            margin: 5px 0 0;
            padding: 0;

            font-size: 11px;
            font-weight: 400;
            line-height: 15px;

            color: #9ca3af;

            overflow-wrap: anywhere;
        }

        /* Waktu pesan masuk rata kiri */
        .ticket-app .message.received .message-time {
            text-align: left;
        }

        /* Waktu pesan keluar rata kanan */
        .ticket-app .message.sent .message-time {
            text-align: right;
        }

        /* ========================================
           AVATAR
           ======================================== */

        .ticket-app .avatar {
            display: flex;
            align-items: center;
            justify-content: center;

            width: 36px;
            height: 36px;
            min-width: 36px;

            margin-right: 10px;

            border-radius: 50%;
            background: #2563eb;
            color: #ffffff;

            font-size: 12px;
            font-weight: 600;

            flex-shrink: 0;
        }

        /* ========================================
           CHAT REPLY
           ======================================== */

        .ticket-app .chat-reply {
            padding: 16px 20px;
            background: #ffffff;
            border-top: 1px solid #eef2f7;
            flex-shrink: 0;
        }

        .ticket-app .reply-tools {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 12px;
        }

        .ticket-app .reply-tools .btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;

            width: 38px;
            height: 38px;
            padding: 0;

            border: 1px solid #e5e7eb;
            border-radius: 10px;

            background: #ffffff;
            color: #64748b;
        }

        .ticket-app .reply-tools .btn:hover {
            background: #f1f5f9;
        }

        .ticket-app .reply-box {
            display: flex;
            align-items: stretch;
            gap: 10px;
            min-width: 0;
        }

        .ticket-app .reply-box textarea {
            flex: 1;
            min-width: 0;

            height: 80px;
            min-height: 80px;

            padding: 12px 14px;

            border: 1px solid #dbe1ea;
            border-radius: 12px;

            background: #ffffff;
            color: #273142;

            font-family: 'Segoe UI', sans-serif;
            font-size: 14px;
            line-height: 1.5;

            resize: vertical;
            outline: none;
        }

        .ticket-app .reply-box textarea:focus {
            border-color: #86b7fe;
            box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.1);
        }

        .ticket-app .reply-box textarea:disabled {
            background: #f8fafc;
            cursor: not-allowed;
        }

        .ticket-app .btn-send {
            display: flex;
            align-items: center;
            justify-content: center;

            width: 54px;
            min-width: 54px;

            border: none;
            border-radius: 12px;

            background: #2563eb;
            color: #ffffff;

            font-size: 18px;
            cursor: pointer;
        }

        .ticket-app .btn-send:hover {
            background: #1d4ed8;
        }

        .ticket-app .btn-send:disabled {
            opacity: 0.5;
            cursor: not-allowed;
        }

        /* ========================================
           EMPTY CHAT
           ======================================== */

        .ticket-app .empty-chat {
            min-height: 200px;
            height: 100%;

            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: column;

            text-align: center;
            color: #9ca3af;
        }

        .ticket-app .empty-chat i {
            margin-bottom: 12px;
            font-size: 48px;
            color: #cbd5e1;
        }

        .ticket-app .empty-chat div {
            font-size: 14px;
            line-height: 1.6;
        }

        /* ========================================
           SCROLLBAR
           ======================================== */

        .ticket-app .chat-body::-webkit-scrollbar,
        .ticket-app .ticket-list::-webkit-scrollbar {
            width: 6px;
        }

        .ticket-app .chat-body::-webkit-scrollbar-track,
        .ticket-app .ticket-list::-webkit-scrollbar-track {
            background: transparent;
        }

        .ticket-app .chat-body::-webkit-scrollbar-thumb,
        .ticket-app .ticket-list::-webkit-scrollbar-thumb {
            background: #d1d5db;
            border-radius: 10px;
        }

        .ticket-app .chat-body::-webkit-scrollbar-thumb:hover,
        .ticket-app .ticket-list::-webkit-scrollbar-thumb:hover {
            background: #9ca3af;
        }

        /* ========================================
           TABLET
           ======================================== */

        @media (max-width: 992px) {
            .ticket-app .sidebar {
                width: 300px;
                min-width: 300px;
            }

            .ticket-app .message-content {
                max-width: 80%;
            }

            .ticket-app .chat-body {
                padding: 20px 16px;
            }
        }

        /* ========================================
           MOBILE
           ======================================== */

        @media (max-width: 768px) {
            .ticket-app {
                height: calc(100vh - 120px);
                min-height: 450px;
            }

            .ticket-app .sidebar {
                width: 240px;
                min-width: 240px;
            }

            .ticket-app .sidebar-header {
                padding: 14px;
            }

            .ticket-app .sidebar-search {
                padding: 10px;
            }

            .ticket-app .ticket-item {
                padding: 13px;
            }

            .ticket-app .ticket-item.active {
                padding-left: 9px;
            }

            .ticket-app .chat-header {
                padding: 13px;
                gap: 8px;
            }

            .ticket-app .customer-info h5 {
                font-size: 15px;
            }

            .ticket-app .header-actions {
                gap: 6px;
            }

            .ticket-app .chat-body {
                padding: 16px 12px;
            }

            .ticket-app .message-content {
                max-width: 85%;
            }

            .ticket-app .message {
                margin-bottom: 18px;
            }

            .ticket-app .bubble {
                padding: 8px 10px;
                font-size: 14px;
                line-height: 20px;
            }

            .ticket-app .chat-reply {
                padding: 12px;
            }

            .ticket-app .reply-box {
                gap: 8px;
            }

            .ticket-app .reply-box textarea {
                height: 70px;
                min-height: 70px;
                padding: 10px 12px;
            }

            .ticket-app .btn-send {
                width: 46px;
                min-width: 46px;
            }
        }

        /* ========================================
           SMALL MOBILE
           ======================================== */

        @media (max-width: 480px) {
            .ticket-app {
                height: calc(100vh - 100px);
                min-height: 400px;
            }

            .ticket-app .sidebar {
                width: 190px;
                min-width: 190px;
            }

            .ticket-app .sidebar-header h4 {
                font-size: 16px;
            }

            .ticket-app .chat-header {
                align-items: flex-start;
            }

            .ticket-app .header-actions {
                flex-direction: column;
                align-items: flex-end;
            }

            .ticket-app .message-content {
                max-width: 90%;
            }

            .ticket-app .chat-body {
                padding: 14px 10px;
            }

            .ticket-app .message-name {
                font-size: 11px;
            }

            .ticket-app .message-time {
                font-size: 10px;
            }
        }
    </style>
    <div class="ticket-app">
        <div class="sidebar">
            <div class="sidebar-header">
                <h4>Support Tickets</h4>
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary btn-new-ticket" Text="New Ticket" OnClick="btnAdd_Click" />
            </div>
            <div class="sidebar-search">
                <div class="input-group">
                    <span class="input-group-text">
                        <i class="bi bi-search"></i>
                    </span>
                    <input type="text" id="txtTicketSearch" class="form-control" placeholder="Search ticket...">
                </div>
            </div>

            <div class="ticket-list">
            </div>
        </div>
        <div class="chat-panel">
            <div class="chat-header">
                <div class="customer-info">
                    <h5 id="lblTicketNo">Select a ticket</h5>
                </div>
                <div class="header-actions">
                    <span id="lblStatus" class="badge bg-secondary">-</span>
                    <button type="button" id="btnCloseTicket" class="btn btn-outline-danger btn-sm" style="display:none;">
                        <i class="bi bi-check-circle"></i>
                        Close Ticket
                    </button>
                </div>
            </div>
            <div class="chat-body">
                <div class="empty-chat">
                    <i class="bi bi-chat-left-text"></i>
                    <br /><br />
                    <div>Select a ticket to view conversation</div>
                </div>
            </div>
            <div class="chat-reply">
                <div class="reply-tools">
                    <button type="button" class="btn btn-light">
                        <i class="bi bi-paperclip"></i>
                    </button>
                    <button type="button" class="btn btn-light">
                        <i class="bi bi-image"></i>
                    </button>
                    <button type="button" class="btn btn-light">
                        <i class="bi bi-emoji-smile"></i>
                    </button>
                </div>
                <div class="reply-box">
                    <textarea id="txtReply" placeholder="Type your reply here..." disabled></textarea>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        let selectedTicketId = 0;
        let tickets = [];
        let messages = [];
        let isLoadingTickets = false;
        let isLoadingMessages = false;
        let lastTicketSignature = "";
        let lastMessageId = 0;
        let searchText = "";

        $(document).ready(function () {
            loadTickets(false);
            setInterval(function () {
                loadTickets(true);
                if (selectedTicketId > 0) {
                    loadMessages(selectedTicketId,true);
                }
            }, 3000);

            // Search
            $("#txtTicketSearch").on("input", function () {
                searchText = $(this).val().toLowerCase().trim();
                renderTickets();
            });

            // Enter
            $("#txtReply").on("keydown", function (e) {
                if (e.key === "Enter" && !e.shiftKey) {
                    e.preventDefault();
                    sendMessage();
                }
            });

            // Close
            $("#btnCloseTicket").on("click", function () {
                closeTicket();
            });
        });

        // ============================================================
        // AJAX
        // ============================================================

        function ajaxCall(method, data) {
            return $.ajax({
                type: "POST",
                url: "Default.aspx/" + method,
                data: JSON.stringify(data || {}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                cache: false
            });
        }

        // ============================================================
        // LOAD TICKETS
        // ============================================================

        function loadTickets(background) {
            if (isLoadingTickets)
                return;

            isLoadingTickets = true;

            ajaxCall("GetTickets").done(function (response) {
                const result = response.d;

                if (!result.success) {
                    console.error(result.message);
                    return;
                }

                tickets = result.data;

                const signature = JSON.stringify(
                    tickets.map(function (x) {
                        return [
                            x.Id,
                            x.Status,
                            x.LastMessageDate,
                            x.UnreadCount
                        ];
                    })
                );

                if (signature !== lastTicketSignature) {
                    lastTicketSignature = signature;
                    renderTickets();
                }

                // Tidak ada auto-select tiket pertama.
                // Chat hanya ditampilkan setelah user memilih tiket.

                if (selectedTicketId > 0) {
                    const current = tickets.find(function (x) {
                        return x.Id === selectedTicketId;
                    });

                    if (current) {
                        updateTicketHeader(current);
                    }
                }

            }).fail(function (xhr) {
                console.error("GetTickets error:", xhr.responseText);

            }).always(function () {
                isLoadingTickets = false;
            });
        }

        // ============================================================
        // RENDER TICKETS
        // ============================================================

        function renderTickets() {
            const container = $(".ticket-list");
            container.empty();

            const filtered = tickets.filter(function (ticket) {
                if (!searchText)
                    return true;

                return (ticket.TicketNo.toLowerCase().includes(searchText) || ticket.Subject.toLowerCase().includes(searchText) || String(ticket.LoginId).includes(searchText)
                );
            });

            if (filtered.length === 0) {
                container.html(`
                    <div class="text-center text-muted p-4">
                        <i class="bi bi-inbox fs-2"></i>
                        <div class="mt-2">No tickets found</div>
                    </div>
                `);
                return;
            }

            filtered.forEach(function (ticket) {
                const active = ticket.Id === selectedTicketId ? "active" : "";
                let unread = "";

                if (ticket.UnreadCount && ticket.UnreadCount > 0) {
                    unread = `
                        <span class="unread">
                            ${ticket.UnreadCount}
                        </span>
                    `;
                }

                const html = `
                    <div class="ticket-item ${active}" data-ticket-id="${ticket.Id}">
                        <div class="ticket-top">
                            <span class="ticket-id">
                                ${escapeHtml(ticket.TicketNo)}
                            </span>
                            <span class="ticket-time">
                                ${formatRelativeTime(ticket.LastMessageDate || ticket.CreatedDate)}
                            </span>
                        </div>

                        <div class="ticket-subject">
                            ${escapeHtml(ticket.Subject)}
                        </div>

                        <div class="ticket-bottom">
                            <span class="badge ${getStatusClass(ticket.Status)}">
                                ${escapeHtml(ticket.Status)}
                            </span>
                            ${unread}
                        </div>
                    </div>
                `;
                container.append(html);
            });

            // Click
            container.find(".ticket-item").off("click").on("click", function () {
                const ticketId = parseInt($(this).attr("data-ticket-id"));
                selectTicket(ticketId);
            });
        }

        // ============================================================
        // SELECT TICKET
        // ============================================================

        function selectTicket(ticketId) {
            if (!ticketId)
                return;

            selectedTicketId = ticketId;

            lastMessageId = 0;

            $(".ticket-item").removeClass("active");
            $('.ticket-item[data-ticket-id="' + ticketId + '"]').addClass("active");
            const ticket = tickets.find(function (x) {
                return x.Id === ticketId;
            });

            if (ticket) {
                updateTicketHeader(ticket);
            }

            $("#txtReply").prop("disabled", false);

            loadMessages(ticketId, false);
        }

        // ============================================================
        // LOAD MESSAGES
        // ============================================================

        function loadMessages(ticketId,background) {
            if (!ticketId || isLoadingMessages)
                return;
            isLoadingMessages = true;

            ajaxCall("GetMessages", { ticketId: ticketId }).done(function (response) {
                const result = response.d;
                if (!result.success) {
                    console.error(result.message);
                    return;
                }

                const newMessages = result.data;
                let latestId = 0;

                if (newMessages.length > 0) {
                    latestId = newMessages[newMessages.length - 1].Id;
                }

                if (!background) {
                    messages = newMessages;

                    renderMessages(messages, true);
                    lastMessageId = latestId;
                    return;
                }

                if (latestId > lastMessageId) {
                    messages = newMessages;

                    renderMessages(messages, true);
                    lastMessageId = latestId;
                }
            }).fail(function (xhr) {
                console.error("GetMessages error:", xhr.responseText);
            }).always(function () {
                isLoadingMessages = false;
            });
        }

        function renderMessages(data, scrollBottom) {
            const body = $(".chat-body");
            if (!data || data.length === 0) {
                body.html(`
                    <div class="empty-chat">
                        <i class="bi bi-chat-left-text"></i>
                        <div>No messages yet.</div>
                    </div>
                `);
                return;
            }

            body.empty();

            let lastDate = "";

            data.forEach(function (message) {
                const date = parseDate(message.CreatedDate);
                const dateKey = date.toLocaleDateString();

                if (dateKey !== lastDate) {
                    body.append(`
                        <div class="chat-date">
                            ${escapeHtml(dateKey)}
                        </div>
                    `);
                    lastDate = dateKey;
                }

                const side = message.IsMine ? "sent" : "received";

                const html = `
                    <div class="message ${side}">
                        <div class="message-content">
                            <div class="message-name">${escapeHtml(String(message.SenderName))}</div>
                            <div class="bubble">${escapeHtml(message.Message)}</div>
                            <div class="message-time">${formatDateTime(message.CreatedDate)}</div>
                        </div>
                    </div>
                `;

                body.append(html);
            });

            if (scrollBottom) {
                body.scrollTop(body[0].scrollHeight);
            }
        }

        // ============================================================
        // SEND MESSAGE
        // ============================================================

        function sendMessage() {
            if (!selectedTicketId)
                return;

            const textbox = $("#txtReply");
            const message = textbox.val().trim();
            if (!message)
                return;

            ajaxCall("SendMessage", { ticketId: selectedTicketId, message: message }).done(function (response) {
                const result = response.d;
                if (!result.success) {
                    alert(result.message);
                    return;
                }
                textbox.val("");

                loadMessages(selectedTicketId, false);
                loadTickets(true);
            }).fail(function (xhr) {
                console.error(xhr.responseText);
                alert("Unable to send message.");
            }).always(function () {
                textbox.focus();
            });
        }

        // ============================================================
        // CLOSE TICKET
        // ============================================================

        function closeTicket() {
            if (!selectedTicketId)
                return;
            if (!confirm("Are you sure you want to close this ticket?"))
                return;

            ajaxCall("CloseTicket", { ticketId: selectedTicketId }).done(function (response) {
                const result = response.d;
                if (!result.success) {
                    alert(result.message);
                    return;
                }
                loadTickets(false);
                loadMessages(selectedTicketId, false);
            }).fail(function (xhr) {
                console.error(xhr.responseText);
            });
        }

        // ============================================================
        // HEADER
        // ============================================================

        function updateTicketHeader(ticket) {
            $("#lblTicketNo").text(ticket.TicketNo);

            $("#lblStatus").removeClass().addClass("badge " + getStatusClass(ticket.Status)).text(ticket.Status);

            if (ticket.Status === "Closed") {
                $("#btnCloseTicket").hide();
                $("#txtReply").prop("disabled", true);
            }
            else {
                $("#btnCloseTicket").show();
                $("#txtReply").prop("disabled", false);
            }
        }

        // ============================================================
        // STATUS
        // ============================================================

        function getStatusClass(status) {
            switch (status) {
                case "Open":
                    return "bg-warning text-dark";
                case "Pending":
                    return "bg-info text-dark";
                case "Resolved":
                    return "bg-success";
                case "Closed":
                    return "bg-secondary";
                default:
                    return "bg-secondary";
            }
        }

        // ============================================================
        // DATE
        // ============================================================

        function parseDate(value) {
            if (!value)
                return new Date();
            return new Date(value.replace(" ", "T"));
        }

        function formatDateTime(value) {
            const date = parseDate(value);

            return date.toLocaleString(
                [],
                {
                    day: "2-digit",
                    month: "short",
                    year: "numeric",
                    hour: "2-digit",
                    minute: "2-digit"
                }
            );
        }

        function formatRelativeTime(value) {
            if (!value)
                return "";

            const date = parseDate(value);
            const now = new Date();
            const diff = now - date;
            const minutes = Math.floor(diff / 60000);

            if (minutes < 1)
                return "now";

            if (minutes < 60)
                return minutes + "m";

            const hours = Math.floor(minutes / 60);

            if (hours < 24)
                return hours + "h";

            const days = Math.floor(hours / 24);

            if (days < 7)
                return days + "d";

            return date.toLocaleDateString();
        }

        // ============================================================
        // ESCAPE HTML
        // ============================================================

        function escapeHtml(value) {
            if (value === null || value === undefined)
                return "";
            return $("<div>").text(value).html();
        }

        // ============================================================
        // STOP HISTORY RELOAD
        // ============================================================

        window.history.replaceState(null,null,window.location.href);
    </script>
</asp:Content>