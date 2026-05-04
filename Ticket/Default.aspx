<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Ticket_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
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
    <div class="page-content">
        <section class="row mb-3">
            <div class="col-12">
                <div class="row mb-2" runat="server" id="divError">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-12 d-flex justify-content-start flex-wrap gap-2">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="New Ticket" OnClick="btnAdd_Click" />
                <asp:Button runat="server" ID="btnTopic" CssClass="btn btn-danger" Text="Set Topic" OnClick="btnTopic_Click" />
                <asp:Button runat="server" ID="btnRefresh" CssClass="btn btn-info" Text="Refresh" OnClick="btnRefresh_Click" />
            </div>
        </section>
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                <div class="input-group">
                                    <span class="input-group-text">Search : </span>
                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                    <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <asp:Repeater ID="rptChatList" runat="server">
                                        <ItemTemplate>
                                            <div class='list-group-item list-group-item-action <%# If(lblid.Text = Eval("Id").ToString(), "bg-primary text-white", "") %>' style="cursor:pointer;" onclick="location.href='/ticket?tid=<%# Eval("Id") %>'">
                                                <div class="d-flex w-100 justify-content-between">
                                                    <h5 class='mb-3 <%# If(lblid.Text = Eval("Id").ToString(), "bg-primary text-white", "") %>'>
                                                        <%# Eval("TopicName") %>
                                                    </h5>
                                                    <small><%# Eval("CreatedDate", "{0:dd MMM yyyy HH:mm}") %></small>
                                                </div>
                                                <p>
                                                    Subject : <b><%# Eval("Subject") %></b>
                                                    <br />
                                                    Last Message : <b><%# Eval("LastMessage") %></b>
                                                </p>
                                                <div class="d-flex w-100 justify-content-between">
                                                    <small>Created by <b><%# Eval("LoginName") %></b></small>

                                                    <small>Status : <b><%# Eval("DataStatus") %></b></small>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-header">
                        <div class="media d-flex align-items-center">
                            <div class="name flex-grow-1">
                                <h6 class="mb-0" runat="server" id="hSubject"></h6>
                                <span class="text-xs" runat="server" id="spanStatus"></span>
                            </div>
                        </div>
                    </div>
                    <div id="chatBody" class="card-body pt-4 bg-grey">
                        <div class="chat-content">
                            <asp:Repeater ID="rptTicketDetail" runat="server">
                                <ItemTemplate>
                                    <div class='chat <%# If(Convert.ToString(Eval("SenderId")) = Convert.ToString(Session("LoginId")), "", "chat-left") %>'>
                                        <div class="chat-body">
                                            <div class="chat-message">
                                                <%# Eval("MessageText") %>
                                                <%# If(Convert.ToString(Eval("SenderId")) <> Convert.ToString(Session("LoginId")), "<div style='font-size:12px; margin-top:5px; opacity:0.7;'>Sender : " & Eval("LoginName") & "</div>", "") %>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                    <div class="card-footer">
                        <div class="message-form d-flex align-items-center">
                            <div class="d-flex">
                                    <asp:Panel runat="server" DefaultButton="btnSend">
                                        <asp:TextBox runat="server" ID="txtMessage" CssClass="form-control" autocomplete="off" placeholder="Type your message.."></asp:TextBox>
                                        <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="btn btn-primary ms-2" OnClick="btnSend_Click" style="display:none;" />
                                    </asp:Panel>
                                    
                                    <button type="button" class="btn btn-primary ms-2 d-flex align-items-center justify-content-center" style="height:38px; width:42px; padding:0;" onclick="document.getElementById('fuMessage').click();">
                                        <i class="bi bi-paperclip"></i>
                                    </button>
                                    
                                    <asp:FileUpload runat="server" ID="fuMessage" ClientIDMode="Static" style="opacity:0; position:absolute; width:0; height:0;" onchange="__doPostBack('<%= btnUpload.UniqueID %>', '')" />
                                    <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" style="display:none;" />
                                </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">New Ticket</h4>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Topic</label>
                            <asp:DropDownList runat="server" ID="ddlTopic" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Subject</label>
                            <asp:TextBox runat="server" ID="txtSubject" CssClass="form-control" placeholder="Subject ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcess"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcess" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcess_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalAttachment" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 runat="server" class="modal-title" id="titleProcess"></h4>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="div1">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="Span1"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="Button1" CssClass="btn btn-primary" Text="Submit" />
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        function showProcess() {
            $("#modalProcess").modal("show");
        }

        ["modalProcess", "modalAttachment"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
