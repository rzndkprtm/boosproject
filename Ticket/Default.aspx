<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Ticket_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>

    * {
        margin: 0;
        padding: 0;
        box-sizing: border-box;
    }

    body {
        background: #f3f6fb;
        font-family: 'Segoe UI', sans-serif;
    }

    .ticket-app {
        height: calc(100vh - 150px);
        min-height: 600px;
        display: flex;
        background: #fff;
        border: 1px solid #e5e7eb;
        border-radius: 10px;
        overflow: hidden;
    }

    /* ================================
       SIDEBAR
       ================================ */

    .sidebar {
        width: 380px;
        min-width: 380px;
        background: #fff;
        border-right: 1px solid #e5e7eb;
        display: flex;
        flex-direction: column;
    }

    .sidebar-header {
        padding: 20px;
        border-bottom: 1px solid #eef2f7;
    }

    .sidebar-header h4 {
        margin-bottom: 15px;
        font-weight: 700;
    }

    .btn-new-ticket {
        width: 100%;
    }

    .sidebar-search {
        padding: 15px;
        border-bottom: 1px solid #eef2f7;
    }

    .ticket-list {
        flex: 1;
        overflow-y: auto;
    }

    .ticket-item {
        padding: 18px;
        border-bottom: 1px solid #f3f4f6;
        cursor: pointer;
        transition: .2s;
    }

    .ticket-item:hover {
        background: #f8fafc;
    }

    .ticket-item.active {
        background: #eff6ff;
        border-left: 4px solid #0d6efd;
        padding-left: 14px;
    }

    .ticket-top {
        display: flex;
        justify-content: space-between;
        margin-bottom: 8px;
    }

    .ticket-id {
        font-weight: 700;
    }

    .ticket-time {
        font-size: 12px;
        color: #999;
    }

    .ticket-subject {
        color: #555;
        font-size: 14px;
        margin-bottom: 10px;
    }

    .ticket-bottom {
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .unread {
        min-width: 22px;
        height: 22px;
        padding: 0 6px;
        border-radius: 50px;
        background: #dc3545;
        color: white;
        font-size: 12px;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    /* ================================
       CHAT
       ================================ */

    .chat-panel {
        flex: 1;
        min-width: 0;
        display: flex;
        flex-direction: column;
    }

    .chat-header {
        background: #fff;
        padding: 20px 25px;
        border-bottom: 1px solid #eef2f7;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .customer-info h5 {
        margin: 0;
        font-weight: 700;
    }

    .customer-info small {
        color: #777;
    }

    .header-actions {
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .chat-body {
        flex: 1;
        overflow-y: auto;
        padding: 30px;
        background: #f8fafc;
    }

    .chat-date {
        text-align: center;
        margin-bottom: 25px;
        color: #999;
        font-size: 13px;
    }

    .message {
        display: flex;
        margin-bottom: 25px;
    }

    .message.sent {
        justify-content: flex-end;
    }

    .avatar {
        width: 42px;
        height: 42px;
        border-radius: 50%;
        background: #2563eb;
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 13px;
        font-weight: 600;
        margin-right: 12px;
        flex-shrink: 0;
    }

    /*
       Message content hanya mengikuti isi bubble.
       Tidak dibuat full width.
    */
    .message-content {
        width: auto;
        max-width: 70%;
        min-width: 0;
        flex: 0 1 auto;
    }

    .message.sent .message-content {
        text-align: right;
    }

    .message-name {
        font-size: 13px;
        color: #777;
        margin-bottom: 5px;
    }

    /*
       Bubble mengikuti panjang dan tinggi teks.
    */
    .bubble {
        display: inline-block;
        width: auto;
        max-width: 100%;
        height: auto;
        min-height: 0;
        padding: 12px 16px;
        border-radius: 16px;
        word-break: break-word;
        overflow-wrap: anywhere;
        white-space: pre-wrap;
        text-align: left;
        vertical-align: top;
        box-sizing: border-box;
    }

    .received .bubble {
        background: #fff;
        border: 1px solid #e5e7eb;
    }

    .sent .bubble {
        background: #2563eb;
        color: #fff;
    }

    .message-time {
        margin-top: 5px;
        font-size: 12px;
        color: #999;
    }

    /* ================================
       REPLY
       ================================ */

    .chat-reply {
        background: #fff;
        border-top: 1px solid #eef2f7;
        padding: 20px;
    }

    .reply-tools {
        display: flex;
        gap: 10px;
        margin-bottom: 15px;
        align-items: center;
    }

    .reply-box {
        display: flex;
        gap: 10px;
    }

    .reply-box textarea {
        flex: 1;
        border: 1px solid #dbe1ea;
        border-radius: 14px;
        resize: none;
        height: 90px;
        padding: 15px;
        outline: none;
    }

    .reply-box textarea:focus {
        border-color: #86b7fe;
        box-shadow: 0 0 0 .15rem rgba(13,110,253,.1);
    }

    .btn-send {
        width: 65px;
        border: none;
        border-radius: 14px;
        background: #0d6efd;
        color: #fff;
        font-size: 20px;
        transition: .3s;
    }

    .btn-send:hover {
        background: #0b5ed7;
    }

    .btn-send:disabled {
        opacity: .6;
        cursor: not-allowed;
    }

    /* ================================
       EMPTY CHAT
       ================================ */

    .empty-chat {
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        color: #999;
    }

    .empty-chat i {
        font-size: 60px;
        margin-bottom: 15px;
    }

    /* ================================
       MOBILE
       ================================ */

    @media (max-width: 768px) {

        .ticket-app {
            height: calc(100vh - 120px);
        }

        .sidebar {
            width: 300px;
            min-width: 300px;
        }

        .message-content {
            max-width: 85%;
        }

        .chat-body {
            padding: 20px;
        }

    }

    ::-webkit-scrollbar {
        width: 8px;
    }

    ::-webkit-scrollbar-thumb {
        background: #d1d5db;
        border-radius: 10px;
    }

</style>
    
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">Support Ticket</p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item">
                                <a runat="server" href="~/">Home</a>
                            </li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="ticket-app">
        <div class="sidebar">
            <div class="sidebar-header">
                <h4>Support Tickets</h4>
                <button type="button" class="btn btn-primary btn-new-ticket">
                    <i class="bi bi-plus-lg"></i>
                    New Ticket
                </button>
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
                    <small id="lblCustomer"></small>
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
                    <button type="button" id="btnSend" class="btn-send" disabled>
                        <i class="bi bi-send-fill"></i>
                    </button>
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

            // Send
            $("#btnSend").on("click", function () {
                sendMessage();
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

                const signature = JSON.stringify(tickets.map(function (x) {
                    return [x.Id, x.Status, x.LastMessageDate, x.UnreadCount];
                })
                );

                if (signature !== lastTicketSignature) {
                    lastTicketSignature = signature;
                    renderTickets();
                }

                if (selectedTicketId === 0 && tickets.length > 0) {
                    selectTicket(tickets[0].Id);
                }

                const current = tickets.find(function (x) {
                    return x.Id === selectedTicketId;
                });

                if (current) {
                    updateTicketHeader(current);
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
                        <div class="mt-2">
                            No tickets found
                        </div>
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
                    <div class="ticket-item ${active}"
                         data-ticket-id="${ticket.Id}">

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
            $("#btnSend").prop("disabled", false);

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
                        <i class="
                            bi bi-chat-left-text
                        "></i>
                        <div>
                            No messages yet.
                        </div>
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
                const senderName = message.SenderType === "Customer" ? "Customer" : "Internal";

                const html = `
                    <div class="message ${side}">
                        <div class="message-content">
                            <div class="message-name">
                                ${senderName}
                                #${message.SenderId}
                            </div>
                            <div class="bubble">
                                ${escapeHtml(message.Message)}
                            </div>

                            <div class="message-time">
                                ${formatDateTime(message.CreatedDate)}
                            </div>
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


            const button = $("#btnSend");
            button.prop("disabled", true);

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
                button.prop("disabled", false);
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

            $("#lblCustomer").text("Login ID: " + ticket.LoginId);

            $("#lblStatus").removeClass().addClass("badge " + getStatusClass(ticket.Status)).text(ticket.Status);

            if (ticket.Status === "Closed") {
                $("#btnCloseTicket").hide();
                $("#txtReply").prop("disabled", true);
                $("#btnSend").prop("disabled", true);
            }
            else {
                $("#btnCloseTicket").show();
                $("#txtReply").prop("disabled", false);
                $("#btnSend").prop("disabled", false);
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