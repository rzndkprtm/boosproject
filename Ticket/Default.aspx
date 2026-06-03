<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Ticket_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        *{ margin:0; padding:0; box-sizing:border-box; }
        body{ background:#f3f6fb; font-family:'Segoe UI',sans-serif; }
        .ticket-app{ height:100vh; display:flex; }
        .sidebar{ width:380px; background:#fff; border-right:1px solid #e5e7eb; display:flex; flex-direction:column; }
        .sidebar-header{ padding:20px; border-bottom:1px solid #eef2f7; }
        .sidebar-header h4{ margin-bottom:15px; font-weight:700; }
        .btn-new-ticket{ width:100%; }
        .sidebar-search{ padding:15px; border-bottom:1px solid #eef2f7; }
        .ticket-list{ flex:1; overflow-y:auto; }
        .ticket-item{ padding:18px; border-bottom:1px solid #f3f4f6; cursor:pointer; transition:.25s; }
        .ticket-item:hover{ background:#f8fafc; }
        .ticket-item.active{ background:#eff6ff; border-left:4px solid #0d6efd; }
        .ticket-top{ display:flex; justify-content:space-between; margin-bottom:8px; }
        .ticket-id{ font-weight:700; }
        .ticket-time{ font-size:12px; color:#999; }
        .ticket-subject{ color:#555; font-size:14px; margin-bottom:10px; }
        .ticket-bottom{ display:flex; justify-content:space-between; align-items:center; }
        .unread{ width:22px; height:22px; border-radius:50%; background:#dc3545; color:white; font-size:12px; display:flex; align-items:center; justify-content:center; }
        .chat-panel{ flex:1; display:flex; flex-direction:column; }
        .chat-header{ background:#fff; padding:20px 25px; border-bottom:1px solid #eef2f7; display:flex; justify-content:space-between; align-items:center; }
        .customer-info h5{ margin:0; font-weight:700; }
        .customer-info small{ color:#777; }
        .header-actions{ display:flex; align-items:center; gap:10px; }
        .chat-body{ flex:1; overflow-y:auto; padding:30px; background:#f8fafc; }
        .chat-date{ text-align:center; margin-bottom:25px; color:#999; font-size:13px; }
        .message{ display:flex; margin-bottom:25px; }
        .message.sent{ justify-content:flex-end; }
        .avatar{ width:42px; height:42px; border-radius:50%; background:#2563eb; color:#fff; display:flex; align-items:center; justify-content:center; font-size:13px; font-weight:600; margin-right:12px; }
        .message-content{ max-width:70%; }
        .message.sent .message-content{ text-align:right; }
        .message-name{ font-size:13px; color:#777; margin-bottom:5px; }
        .bubble{ display:inline-block; padding:12px 16px; border-radius:16px; max-width:100%; word-break:break-word; }
        .received .bubble{ background:#fff; border:1px solid #e5e7eb; }
        .sent .bubble{ background:#2563eb; color:#fff; }
        .message-time{ margin-top:5px; font-size:12px; color:#999; }
        .attachment{ margin-top:10px; background:#fff; border:1px solid #e5e7eb; border-radius:12px; padding:12px; display:flex; align-items:center; gap:12px; }
        .attachment i{ font-size:30px; color:#dc3545; }
        .file-title{ font-weight:600; }
        .chat-reply{ background:#fff; border-top:1px solid #eef2f7; padding:20px; }
        .reply-tools{ display:flex; gap:10px; margin-bottom:15px; align-items:center; }
        .reply-box{ display:flex; gap:10px; }
        .reply-box textarea{ flex:1; border:1px solid #dbe1ea; border-radius:14px; resize:none; height:90px; padding:15px; outline:none; }
        .btn-send{ width:65px; border:none; border-radius:14px; background:#0d6efd; color:#fff; font-size:20px; transition:.3s; }
        .btn-send:hover{ background:#0b5ed7; }
        ::-webkit-scrollbar{ width:8px; }
        ::-webkit-scrollbar-thumb{ background:#d1d5db; border-radius:10px; }
    </style>
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">UNDER CONSTRUCTION !!!!</p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
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
                <button class="btn btn-primary btn-new-ticket">New Ticket</button>
            </div>
            <div class="sidebar-search">
                <div class="input-group">
                    <span class="input-group-text">
                        <i class="bi bi-search"></i>
                    </span>
                    <input type="text" class="form-control" placeholder="Search ticket...">
                </div>
            </div>
            <div class="ticket-list">
                <div class="ticket-item active">
                    <div class="ticket-top">
                        <span class="ticket-id">#TKT-1001</span>
                        <span class="ticket-time">2m</span>
                    </div>
                    <div class="ticket-subject">Need quotation update</div>
                    <div class="ticket-bottom">
                        <span class="badge bg-warning text-dark">Pending</span>
                        <span class="unread">3</span>
                    </div>
                </div>
            </div>
        </div>
        <div class="chat-panel">
            <div class="chat-header">
                <div class="customer-info">
                    <h5>#TKT-1001</h5>
                    <small>John Smith</small>
                </div>
                <div class="header-actions">
                    <span class="badge bg-warning text-dark">Pending</span>
                    <button class="btn btn-outline-danger btn-sm">
                        <i class="bi bi-check-circle"></i>
                        Close Ticket
                    </button>
                </div>
            </div>
            <div class="chat-body">
                <div class="chat-date">Today</div>
                <div class="message received">
                    <div class="avatar">JS</div>
                    <div class="message-content">
                        <div class="message-name">John Smith</div>
                        <div class="bubble">Hi team, I need help with my quotation.</div>
                        <div class="message-time">10:15 AM</div>
                    </div>
                </div>
                <div class="message sent">
                    <div class="message-content">
                        <div class="message-name">Support Team</div>
                        <div class="bubble">Sure. Could you provide more information?</div>
                        <div class="message-time">10:17 AM</div>
                    </div>
                </div>
                <div class="message received">
                    <div class="avatar">JS</div>
                    <div class="message-content">
                        <div class="message-name">John Smith</div>
                        <div class="bubble">Please see the attached document.</div>
                        <div class="attachment">
                            <i class="bi bi-file-earmark-pdf-fill"></i>
                            <div>
                                <div class="file-title">Quotation.pdf</div>
                                <small>245 KB</small>
                            </div>
                            <button class="btn btn-sm btn-light">Download</button>
                        </div>
                        <div class="message-time">10:20 AM</div>
                    </div>
                </div>
            </div>
            <div class="chat-reply">
                <div class="reply-tools">
                    <button class="btn btn-light">
                        <i class="bi bi-paperclip"></i>
                    </button>
                    <button class="btn btn-light">
                        <i class="bi bi-image"></i>
                    </button>
                    <button class="btn btn-light">
                        <i class="bi bi-emoji-smile"></i>
                    </button>
                    <input type="file" class="form-control">
                </div>
                <div class="reply-box">
                    <textarea placeholder="Type your reply here..."></textarea>
                    <button class="btn-send">
                        <i class="bi bi-send-fill"></i>
                    </button>
                </div>
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
