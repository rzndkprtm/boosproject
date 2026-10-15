<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_General_Ticket_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .info-item { height: 100%; }
        .info-label { font-size: 0.78rem; font-weight: 600; color: #6c757d; text-transform: uppercase; letter-spacing: 0.03em; margin-bottom: 7px; }
        .info-value { font-size: 1rem; font-weight: 600; color: #212529; min-height: 24px; }
        .info-description { background: #f8f9fa; border: 1px solid #edf0f2; border-radius: 6px; padding: 12px 14px; min-height: 45px; color: #495057; line-height: 1.5; }
        .card { border-radius: 8px; }
        .card-header { border-radius: 8px 8px 0 0 !important; }
        .card-title { color: #212529; }
        .card-footer .btn { min-width: 90px; }
        @media (max-width: 767.98px) {
            .card-header,
            .card-body,
            .card-footer { padding-left: 16px !important; padding-right: 16px !important; }
            .card-footer .d-flex { justify-content: stretch !important; }
            .card-footer .btn { flex: 1; }
        }
    </style>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general">General</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general/ticket">Ticket</a></li>
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
        <section class="row g-3">
            <div class="col-12">
                <div class="card border-0 shadow-sm">
                    <div class="card-header bg-white border-bottom py-3 px-4">
                        <div class="d-flex align-items-center justify-content-between">
                            <div>
                                <h5 class="card-title mt-2 mb-1 fw-semibold">Ticket Information</h5>
                                <small class="text-muted">Ticket details</small>
                            </div>
                            <div class="text-muted small">
                                <span class="me-1">ID:</span>
                                <asp:Label runat="server" ID="lblId" CssClass="fw-semibold"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="card-body px-4 py-4">
                        <div class="row g-4">
                            <div class="col-12 col-md-6 col-lg-3">
                                <div class="info-item">
                                    <div class="info-label">Ticket No</div>
                                    <div class="info-value">
                                        <asp:Label runat="server" ID="lblTicketNo"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <div class="info-item">
                                    <div class="info-label">Type</div>
                                    <div class="info-value">
                                        <asp:Label runat="server" ID="lblType"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <div class="info-item">
                                    <div class="info-label">Customer Name</div>
                                    <div class="info-value">
                                        <asp:Label runat="server" ID="lblCustomerName"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-6 col-lg-3">
                                <div class="info-item">
                                    <div class="info-label">Created By</div>
                                    <div class="info-value">
                                        <asp:Label runat="server" ID="lblCreatedFullName" CssClass="fw-semibold"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12">
                                <div class="info-item">
                                    <div class="info-label">Subject</div>
                                    <div class="info-description">
                                        <asp:Label runat="server" ID="lblSubject"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer bg-light border-top px-4 py-3">
                        <div class="d-flex justify-content-end align-items-center flex-wrap gap-2">
                            <a href="javascript:void(0);" runat="server" id="aDelete" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <div class="d-flex align-items-center justify-content-between flex-wrap gap-2">
                            <div><h5 class="card-title mb-1 fw-semibold">List Message</h5></div>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" />
                                    <asp:BoundField DataField="MessageType" HeaderText="Message Type" />
                                    <asp:BoundField DataField="SenderType" HeaderText="Sender Type" />
                                    <asp:BoundField DataField="SenderName" HeaderText="Sender Name" />
                                    <asp:BoundField DataField="Message" HeaderText="Message" />
                                    <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy HH:mm:ss}" />
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                        <ItemTemplate>
                                            <a href="javascript:void(0);" class="btn btn-sm btn-danger" data-bs-toggle="modal" data-bs-target="#modalDeleteDetail" onclick='<%# String.Format("return dataDeleteDetail(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Ticket</h5>
                </div>
                <div class="modal-body">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDelete_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Detail Ticket</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtDeleteDetailId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteDetail" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteDetail_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function dataDeleteDetail(ticketDetailId) {
            document.getElementById("<%=txtDeleteDetailId.ClientID %>").value = ticketDetailId;
        }
        ["modalDelete", "modalDeleteDetail"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>