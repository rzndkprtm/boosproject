<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Packing.aspx.vb" Inherits="Order_Packing" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Order Packing List" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .invoice-card { border: 1px solid #e9ecef; border-radius: 10px; overflow: hidden; }
        .invoice-card .card-header { background-color: #fff; border-bottom: 1px solid #e9ecef; padding: 1.25rem 1.5rem; }
        .invoice-card .card-title { margin-bottom: 0; font-size: 1.1rem; font-weight: 600; }
        .invoice-card .card-body { padding: 1.5rem; }
        .invoice-form-section { background-color: #f8f9fa; border: 1px solid #e9ecef; border-radius: 8px; padding: 1rem; margin-bottom: 1.5rem; }
        .invoice-form-section .form-label { font-weight: 600; margin-bottom: 0.5rem; color: #344054; }
        .invoice-table { margin-bottom: 0; vertical-align: middle; }
        .invoice-table th { background-color: #f8f9fa; color: #344054; font-size: 0.85rem; font-weight: 600; white-space: nowrap; padding: 0.85rem 1rem; border-bottom: 1px solid #dee2e6; }
        .invoice-table td { padding: 0.8rem 1rem; vertical-align: middle; }
        .invoice-table tbody tr:hover { background-color: #f8faff; }
        .invoice-table input[type="checkbox"] { width: 16px; height: 16px; cursor: pointer; vertical-align: middle; }
        .invoice-card .card-footer { background-color: #fff; border-top: 1px solid #e9ecef; padding: 1rem 1.5rem; }
        .invoice-actions { display: flex; justify-content: flex-start; align-items: center; gap: 0.75rem; flex-wrap: wrap; }
        .invoice-actions .btn { min-width: 150px; font-weight: 500; padding: 0.65rem 1rem; border-radius: 6px; }
        .invoice-empty { text-align: center; color: #98a2b3; padding: 1.5rem !important; }
        @media (max-width: 576px) { .invoice-card .card-header, .invoice-card .card-body, .invoice-card .card-footer { padding: 1rem; } .invoice-actions { flex-direction: column; align-items: stretch; } .invoice-actions .btn { width: 100%; } }
    </style>
    <div class="page-heading">
        <div class="page-title">
            <div class="row align-items-center">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted mb-0">Manage and generate order invoices.</p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/order">Order</a></li>
                            <li class="breadcrumb-item"><a href='<%= ResolveUrl("~/order/detail?orderid=" & lblHeaderId.Text) %>'>Detail</a></li>
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
                        <div class="alert alert-danger mb-0" role="alert">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card invoice-card">
                    <div class="card-header">
                        <div class="d-flex align-items-center">
                            <div>
                                <h4 class="card-title">Custom Invoice Form</h4>
                                <small class="text-muted">Enter the invoice number and select the items to include.</small>
                            </div>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="invoice-form-section mt-3">
                            <div class="row">
                                <div class="col-12 col-md-6 col-lg-5">
                                    <label class="form-label" for="<%= txtInvoiceNumber.ClientID %>">Invoice Number</label>
                                    <div class="input-group">
                                        <asp:TextBox runat="server" ID="txtInvoiceNumber" CssClass="form-control" placeholder="Enter invoice number..." autocomplete="off"></asp:TextBox>
                                        <a class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalInfo"> ? </a>
                                    </div>
                                    <small class="text-muted d-block mt-1">
                                        Enter the invoice number to be used for this document.
                                    </small>
                                </div>
                            </div>
                        </div>
                        <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
                            <div>
                                <h6 class="mb-1 fw-bold">Invoice Items</h6>
                                <small class="text-muted">Select the items you want to include in the invoice.</small>
                            </div>
                        </div>
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover invoice-table" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" ShowHeaderWhenEmpty="true" EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataRowStyle-CssClass="invoice-empty" DataKeyNames="Id">
                                <Columns>
                                    <asp:TemplateField HeaderText="SELECT" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="75px" ItemStyle-Width="75px">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" Checked="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="NO" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="65px" ItemStyle-Width="65px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100px" />
                                    <asp:TemplateField HeaderText="DESCRIPTION">
                                        <ItemTemplate>
                                            <%# BindProductDescription(Eval("Id")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="card-footer">
                        <div class="invoice-actions">
                            <asp:Button runat="server" ID="btnDownloadInvoice" CssClass="btn btn-primary" Text="Download Invoice" OnClick="btnDownloadInvoice_Click" />
                            <asp:Button runat="server" ID="btnPreviewInvoice" CssClass="btn btn-info" Text="Preview Invoice" OnClick="btnPreviewInvoice_Click" />
                            <a href="javascript:void(0);" runat="server" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalSendInvoice">Send Invoice</a>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalSendInvoice" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Send Invoice</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">To</label>
                            <asp:TextBox runat="server" ID="txtSendInvoiceTo" CssClass="form-control" placeholder="Customer Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC Customer</label>
                            <asp:TextBox runat="server" ID="txtSendInvoiceCCCustomer" TextMode="MultiLine" CssClass="form-control" Height="135px" placeholder="CC Customer ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            <p><small class="text-muted">* Please enter the required email address in a new line.</small></p>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC Staff</label>
                            <asp:TextBox runat="server" ID="txtSendInvoiceCCStaff" TextMode="MultiLine" CssClass="form-control"  Height="135px" placeholder="CC ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            <p><small class="text-muted">* Please enter the required email address in a new line.</small></p>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSendInvoice" CssClass="btn btn-primary" Text="Submit" OnClick="btnSendInvoice_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalInfo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Information</h5>
                </div>
                <div class="modal-body">
                    <b>PLEASE NOTE</b>:<br /><br />
                    If you change the Invoice Number and then proceed with downloading or previewing the invoice, the Invoice Number will be automatically updated in the system database.
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyId"></asp:Label>
        <asp:Label runat="server" ID="lblCustomerName"></asp:Label>
        <asp:Label runat="server" ID="lblInvoiceNumber"></asp:Label>
        <asp:Label runat="server" ID="lblOrderStatus"></asp:Label>
    </div>

    <script type="text/javascript">
        ["modalSendInvoice", "modalInfo"].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>