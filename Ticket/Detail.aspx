<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Ticket_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Timer runat="server" ID="tmrTicketDetail" Interval="1000" OnTick="tmrTicketDetail_Tick" />

    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/ticket">Ticket</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
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
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7 order-md-1 order-last">
                <div class="card">
                    <asp:UpdatePanel runat="server" ID="upDetail" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-header">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                        <h4 class="card-title">Ticket History</h4>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                        <a href="#" runat="server" id="aReply" class="btn btn-primary me-2" data-bs-toggle="modal" data-bs-target="#modalReply">Reply</a>
                                        <a href="#" runat="server" id="aClose" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalClose">Close</a>
                                    </div>
                                </div>
                            </div>
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="list-group">
                                        <asp:Repeater runat="server" ID="rptDetail" OnItemDataBound="rptDetail_ItemDataBound">
                                            <ItemTemplate>
                                                <div class="list-group-item list-group-item-action">
                                                    <div class="d-flex w-100 justify-content-between">
                                                        <h5 class="mb-3"><%# Eval("ReplyName") %> - <%# Eval("ReplyRole") %></h5>
                                                        <small><%# Eval("CreatedDate", "{0:dd MMM yyyy HH:mm}") %></small>
                                                    </div>

                                                    <p runat="server" id="pMessage" class="mb-3"></p>

                                                    <small runat="server" id="titleAttachment">Attachments :</small>
                                                    <small>
                                                        <ul>
                                                            <asp:Repeater ID="rptAttachment" runat="server">
                                                                <ItemTemplate>
                                                                    <li>
                                                                        <a href='<%# ResolveUrl("~/File/Ticket/" & CType(Container.Parent.Parent, RepeaterItem).DataItem("Id") & "/" & Eval("FileName")) %>'
                                                                           target="_blank">
                                                                           <%# Eval("FileName") %>
                                                                        </a>
                                                                    </li>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </ul>
                                                    </small>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="tmrTicketDetail" EventName="Tick" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5 order-md-2 order-first">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Ticket Data</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-6 form-group">
                                    <label class="form-label">Ticket Code</label>
                                    <asp:TextBox runat="server" ID="txtTicketCode" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-6 form-group">
                                    <label class="form-label">Issue</label>
                                    <asp:TextBox runat="server" ID="txtIssue" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-12 form-group">
                                    <label class="form-label">Subject</label>
                                    <asp:TextBox runat="server" ID="txtSubject" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-6 form-group">
                                    <label class="form-label">Created By</label>
                                    <asp:TextBox runat="server" ID="txtCreatedBy" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-6 form-group">
                                    <label class="form-label">Created Date</label>
                                    <asp:TextBox runat="server" ID="txtCreatedDate" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalReply" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Reply</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Message</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtMessage" Height="200px" CssClass="form-control" placeholder="Message ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Attachment</label>
                            <asp:FileUpload runat="server" ID="fuAttachment" CssClass="form-control" AllowMultiple="true" onchange="showSelectedFiles(this)" />

                            <div id="fileList" style="margin-top:10px; font-family:Cambria; font-size:14px; color:#333;"></div>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorReply">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorReply"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnReply" CssClass="btn btn-primary ml-1" Text="Submit" OnClick="btnReply_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalClose" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Close Ticket</h5>
                </div>

                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnClose" CssClass="btn btn-danger" Text="Confirm" OnClick="btnClose_Click" />
                </div>
            </div>
        </div>
    </div>


    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            document.title = '<%= Page.Title %> - <%= Session("CompanyName") %>';
        });
        function showSelectedFiles(input) {
            const list = document.getElementById("fileList");
            list.innerHTML = "";

            if (input.files.length > 0) {
                const ul = document.createElement("ul");
                ul.style.listStyleType = "disc";
                ul.style.paddingLeft = "20px";

                for (let i = 0; i < input.files.length; i++) {
                    const li = document.createElement("li");
                    li.textContent = input.files[i].name;
                    ul.appendChild(li);
                }

                list.innerHTML = "<b>📎 Files selected:</b>";
                list.appendChild(ul);
            } else {
                list.innerHTML = "<i>No file selected.</i>";
            }
        }

        function showReply() {
            $("#modalReply").modal("show");
        }

        ["modalReply"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
    </script>
</asp:Content>