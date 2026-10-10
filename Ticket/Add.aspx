<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Ticket_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Ticket" %>

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
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Ticket Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row mb-2">
                                    <div class="col-6 form-group">
                                        <label class="form-label">Type</label>
                                        <asp:DropDownList runat="server" ID="ddlType" CssClass="choices form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Pricing" Text="Pricing"></asp:ListItem>
                                            <asp:ListItem Value="Product" Text="Product"></asp:ListItem>
                                            <asp:ListItem Value="Shipment" Text="Shipment"></asp:ListItem>
                                            <asp:ListItem Value="General" Text="General"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Subject</label>
                                        <asp:TextBox runat="server" ID="txtSubject" CssClass="form-control" placeholder="Subject ...." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Message</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtMessage" Height="130px" CssClass="form-control" placeholder="Message ...." autocomplete="off" style="resize: none"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-3" runat="server" id="divError">
                                    <div class="col-12">
                                        <div class="alert alert-danger">
                                            <span runat="server" id="msgError"></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalSuccess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Successfully</h5>
                </div>
                <div class="modal-body text-center">
                    <b>Your ticket has been successfully submitted.</b><br /><br />
                    Your Ticket Reference Number is:<br />
                    <strong id="lblTicketNo"></strong>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" id="btnCloseSuccess" class="btn btn-success w-100">Close</a>
                </div>
            </div>
        </div>
    </div>

    <script>
        document.getElementById("modalSuccess").addEventListener("hide.bs.modal", function () {
            document.activeElement.blur();
            document.body.focus();
        });

        var ticketRedirectTimer;

        function showSuccessModal(ticketNo) {
            document.getElementById("lblTicketNo").textContent = ticketNo;

            var modalElement = document.getElementById("modalSuccess");
            var modal = new bootstrap.Modal(modalElement);

            modal.show();

            ticketRedirectTimer = setTimeout(function () {
                window.location.href = '<%= ResolveUrl("~/ticket") %>';
            }, 5000);
        }

        document.getElementById("btnCloseSuccess").addEventListener("click", function () {
            clearTimeout(ticketRedirectTimer);
            window.location.href = '<%= ResolveUrl("~/ticket") %>';
        });
    </script>
</asp:Content>