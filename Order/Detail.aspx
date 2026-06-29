<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Order_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Order Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .detail-section { margin-bottom: 1.75rem; }
        .detail-title { font-size: .85rem; font-weight: 700; text-transform: uppercase; letter-spacing: .08em; color: #6c757d; margin-bottom: 0; padding-bottom: 0; border-bottom: none; }
        .section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; padding-bottom: .5rem; border-bottom: 1px solid #dee2e6; }
        .detail-row { display: flex; margin-bottom: .75rem; }
        .detail-label { width: 180px; color: #6c757d; flex-shrink: 0; }
        .detail-value { font-weight: 600; word-break: break-word; }
        .note-box { border: 1px solid #dee2e6; border-radius: .375rem; padding: .75rem 1rem; background: #fafafa; min-height: 70px; }
        .timeline-item { margin-bottom: 1rem; }
        .timeline-label { font-size: .75rem; color: #6c757d; text-transform: uppercase; }
        .timeline-value { font-weight: 600; }
        .finance-value { font-size: 1.1rem; font-weight: 700; }
        .finance-highlight { background: #fafafa; border-radius: .5rem; padding: 1rem; }
        .card-header h5 { margin-bottom: 0; }
        .fw-semibold { font-weight: 600; }
        .table > :not(caption) > * > * { vertical-align: middle; }
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
                            <li class="breadcrumb-item"><a runat="server" href="~/order">Order</a></li>
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
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <button class="btn btn-primary dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false">Generate PDF</button>
                <ul class="dropdown-menu">
                    <li><asp:Button runat="server" ID="btnPreview" CssClass="dropdown-item" Text="Preview" /></li>
                    <li><asp:Button runat="server" ID="btnDownload" CssClass="dropdown-item" Text="Download" OnClick="btnDownload_Click" /></li>
                    <li><asp:Button runat="server" ID="btnSuratJalan" CssClass="dropdown-item" Text="Surat Jalan" /></li>
                </ul>
                <asp:Button runat="server" ID="btnEditOrder" CssClass="btn btn-secondary me-1" Text="Edit Order" OnClick="btnEditOrder_Click" />
                <a href="javascript:void(0);" runat="server" id="aDeleteOrder" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalDeleteOrder">Delete Order</a>
                <a href="javascript:void(0);" runat="server" id="aQuoteOrder" class="btn btn-info me-1" data-bs-toggle="modal" data-bs-target="#modalQuoteOrder">Quote Order</a>
                <a href="javascript:void(0);" runat="server" id="aSubmitOrder" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalSubmitOrder">Submit Order</a>
                <a href="javascript:void(0);" runat="server" id="aDuplicateOrder" class="btn btn-warning me-1" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder">Duplicate Order</a>
                <button class="btn btn-info dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnUpdateStatus">Update Status</button>
                <ul class="dropdown-menu">
                    <li><a href="javascript:void(0);" runat="server" id="aNewOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalNewOrder">New Order</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aUnsubmitOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalUnsubmitOrder">Unsubmit Order</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aProductionOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalProductionOrder">Production Order</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aHoldOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalHoldOrder">Hold Order</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aShippedOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalShippedOrder">Shipped Order</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aCompleteOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalCompleteOrder">Complete Order</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aCancelOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalCancelOrder">Cancel Order</a></li>
                </ul>
                <a href="javascript:void(0);" runat="server" id="aReworkOrder" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalReworkOrder">Rework Order</a>
                <button class="btn btn-info dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnQuoteAction">Quote</button>
                <ul class="dropdown-menu">
                    <li><a href="javascript:void(0);" runat="server" id="aQuoteCustomer" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDetailQuote">Quote Details</a></li>
                    <li><a href="javascript:void(0);" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDownloadQuote">Download Quote</a></li>
                    <li><asp:Button runat="server" ID="btnPreviewQuote" CssClass="dropdown-item" Text="Preview Quote" /></li>
                    <li><a href="javascript:void(0);" runat="server" id="aSendQuote" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalSendQuote">Send Quote</a></li>
                </ul>
                <button class="btn btn-primary dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnInvoice">invoice</button>
                <ul class="dropdown-menu">
                    <li><a href="javascript:void(0);" runat="server" id="aSendInvoice" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalSendInvoice">Send Invoice</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aReceivePayment" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalReceivePayment">Receive Payment</a></li>
                    <li><asp:Button runat="server" ID="btnPreviewInvoice" CssClass="dropdown-item" Text="Preview Invoice" /></li>
                    <li><a href="javascript:void(0);" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDownloadInvoice">Download Invoice (PDF)</a></li>
                    <li><a href="javascript:void(0);" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDownloadInvoiceCSV">Download Invoice (CSV)</a></li>
                    <li runat="server" id="liDividerInvoice"><hr class="dropdown-divider"></li>
                    <li><a href="javascript:void(0);" runat="server" id="aUpdateInvoiceNumber" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalInvoiceNumber">Update Invoice Number</a></li>
                    <li><a href="javascript:void(0);" runat="server" id="aUpdateInvoiceData" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalInvoiceData">Update Invoice Data</a></li>
                </ul>
                <a href="javascript:void(0);" runat="server" id="aBuilderData" class="btn btn-primary me-1" data-bs-toggle="modal" data-bs-target="#modalBuilderDetail">Builder Data</a>
                <a href="javascript:void(0);" runat="server" id="aFile" class="btn btn-dark me-1" data-bs-toggle="modal" data-bs-target="#modalFileOrder">Files</a>
                <button class="btn btn-primary dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnJob">Job</button>
                <ul class="dropdown-menu">
                    <li>
                        <a href="javascript:void(0);" runat="server" id="aConvertOrder" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalConvertOrder">Convert</a>
                    </li>
                    <li>
                        <a href="javascript:void(0);" runat="server" id="aReConvertJob" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalReConvertOrder">Re-Convert</a>
                    </li>
                    <li>
                        <a href="javascript:void(0);" runat="server" id="aDataJob" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDataJob">Data Job</a>
                    </li>
                    <li>
                        <a href="javascript:void(0);" runat="server" id="aUpdateJob" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalUpdateJobData">Update Job Data</a>
                    </li>
                    <li>
                        <asp:Button runat="server" ID="btnPreviewJob" CssClass="dropdown-item" Text="Preview" />
                    </li>
                    <li>
                        <asp:Button runat="server" ID="btnDownloadJob" CssClass="dropdown-item" Text="Download" OnClick="btnDownloadJob_Click" />
                    </li>
                </ul>
                <a href="javascript:void(0);" runat="server" id="aLog" class="btn btn-secondary me-1" onclick="showLogFromElement(this)">Log</a>
            </div>
        </section>
        <section class="row">
            <div class="col-lg-7">
                <div class="card border-0 shadow-sm">
                    <div class="card-body">
                        <div class="mb-4 pb-3 border-bottom">
                            <div class="d-flex justify-content-between align-items-start">
                                <div>
                                    <h2 class="fw-bold mb-1">
                                        <asp:Label runat="server" ID="lblCustomerName"></asp:Label>
                                    </h2>
                                    <div class="text-muted">Customer Order</div>
                                </div>
                                <div class="text-end">
                                    <div class="small text-muted">ORDER #</div>
                                    <div class="fs-4 fw-bold">
                                        <asp:Label runat="server" ID="lblOrderId"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="detail-section">
                            <div class="detail-row">
                                <div class="detail-label">Order Number</div>
                                <div class="detail-value">
                                    <asp:Label runat="server" ID="lblOrderNumber"></asp:Label>
                                </div>
                            </div>
                            <div class="detail-row">
                                <div class="detail-label">Order Name</div>
                                <div class="detail-value">
                                    <asp:Label runat="server" ID="lblOrderName"></asp:Label>
                                </div>
                            </div>
                            <div class="detail-row">
                                <div class="detail-label">Created By</div>
                                <div class="detail-value">
                                    <asp:Label runat="server" ID="lblCreatedName"></asp:Label>
                                    <asp:Label runat="server" ID="lblCreatedBy" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <div class="detail-row">
                                <div class="detail-label">Order Status</div>
                                <div class="detail-value">
                                    <asp:Label runat="server" ID="lblOrderStatus"></asp:Label>
                                </div>
                            </div>
                            <div class="detail-row" runat="server" id="divOrderType">
                                <div class="detail-label">Order Type</div>
                                <div class="detail-value">
                                    <asp:Label runat="server" ID="lblOrderType"></asp:Label>
                                </div>
                            </div>
                            <div class="detail-row" runat="server" id="divOrderFactory">
                                <div class="detail-label">Order Factory</div>
                                <div class="detail-value">
                                    <asp:Label runat="server" ID="lblOrderFactory"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="detail-section">
                            <div class="detail-title">Order Note</div>
                            <div class="note-box">
                                <asp:Label runat="server" ID="lblOrderNote"></asp:Label>
                            </div>
                        </div>
                        <div runat="server" id="divInternalNote" class="detail-section">
                            <div class="section-header">
                                <div class="detail-title">Internal Note (Latest)</div>
                                <div>
                                    <a href="javascript:void(0);" class="btn btn-sm btn-outline-primary me-1" data-bs-toggle="modal" data-bs-target="#modalAddNote">Add Note</a>
                                    <a href="javascript:void(0);" class="btn btn-sm btn-outline-danger" onclick="showHistoryNote('<%= lblHeaderId.Text %>')">History Note</a>
                                </div>
                            </div>                            
                            <div class="note-box">
                                <asp:Label runat="server" ID="lblInternalNote"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-5">
                <div class="card border-0 shadow-sm mb-3" runat="server" id="divDateOrder">
                    <div class="card-body">
                        <div class="section-header">
                            <div class="detail-title">Order Timeline</div>
                            <div>
                                <a href="javascript:void(0);" runat="server" id="aMoreDownloadBOE" class="btn btn-sm btn-outline-primary" data-bs-toggle="modal" data-bs-target="#modalDownloadBOE">Download BOE</a>
                            </div>
                        </div>
                        <div class="row" runat="server" id="divDateAction">
                            <div class="col-md-6 timeline-item">
                                <div class="timeline-label">Created</div>
                                <div class="timeline-value">
                                    <asp:Label runat="server" ID="lblCreatedDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6 timeline-item">
                                <div class="timeline-label">Submitted</div>
                                <div class="timeline-value">
                                    <asp:Label runat="server" ID="lblSubmittedDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6 timeline-item">
                                <div class="timeline-label">Production</div>
                                <div class="timeline-value">
                                    <asp:Label runat="server" ID="lblProductionDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6 timeline-item">
                                <div class="timeline-label">On Hold</div>
                                <div class="timeline-value">
                                    <asp:Label runat="server" ID="lblOnHoldDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6 timeline-item">
                                <div class="timeline-label">Completed</div>
                                <div class="timeline-value">
                                    <asp:Label runat="server" ID="lblCompletedDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6 timeline-item">
                                <div class="timeline-label">Canceled</div>
                                <div class="timeline-value">
                                    <asp:Label runat="server" ID="lblCanceledDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6 timeline-item">
                                <div class="timeline-label">Quoted</div>
                                <div class="timeline-value">
                                    <asp:Label runat="server" ID="lblQuotedDate"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card border-0 shadow-sm" runat="server" id="divShipmentOrder">
                    <div class="card-body">
                        <div class="section-header">
                            <div class="detail-title">Shipment Information</div>
                        </div>
                        <div class="detail-row">
                            <div class="detail-label">Shipment No</div>
                            <div class="detail-value">
                                <asp:Label runat="server" ID="lblShipmentNumber"></asp:Label>
                            </div>
                        </div>
                        <div class="detail-row">
                            <div class="detail-label">Shipment Date</div>
                            <div class="detail-value">
                                <asp:Label runat="server" ID="lblShipmentDate"></asp:Label>
                            </div>
                        </div>
                        <div class="detail-row">
                            <div class="detail-label">Container No</div>
                            <div class="detail-value">
                                <asp:Label runat="server" ID="lblContainerNumber"></asp:Label>
                            </div>
                        </div>
                        <div class="detail-row">
                            <div class="detail-label">Container ETA</div>
                            <div class="detail-value">
                                <asp:Label runat="server" ID="lblContainerEta"></asp:Label>
                            </div>
                        </div>
                        <div class="detail-row mb-0">
                            <div class="detail-label">Courier</div>
                            <div class="detail-value">
                                <asp:Label runat="server" ID="lblCourier"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row" runat="server" id="secPricing">
            <div class="col-12">
                <div class="card border-0 shadow-sm">
                    <div class="card-body">
                        <div class="section-header">
                            <div class="detail-title">Financial Information</div>
                            <div>
                                <a href="javascript:void(0);" runat="server" id="aRePrice" class="btn btn-sm btn-outline-info" data-bs-toggle="modal" data-bs-target="#modalRePrice">Re Price Order</a>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="detail-section" runat="server" id="divInvoicing">
                                    <div class="enterprise-title mb-2">INVOICING</div>
                                    <div class="detail-row">
                                        <div class="detail-label">Invoice Number</div>
                                        <div class="detail-value">
                                            <asp:Label runat="server" ID="lblInvoiceNumber"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="detail-row">
                                        <div class="detail-label">Invoice Date</div>
                                        <div class="detail-value">
                                            <asp:Label runat="server" ID="lblInvoiceDate"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="detail-row">
                                        <div class="detail-label">Collector</div>
                                        <div class="detail-value">
                                            <asp:Label runat="server" ID="lblCollector"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="detail-row">
                                        <div class="detail-label">Payment Date</div>
                                        <div class="detail-value">
                                            <asp:Label runat="server" ID="lblPaymentDate"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6" runat="server" id="divCosting">
                                <div class="finance-highlight">
                                    <div class="enterprise-title mb-2">COST SUMMARY</div>
                                    <div class="detail-row">
                                        <div class="detail-label">
                                            <asp:Label runat="server" ID="lblPriceOrderTitle"></asp:Label>
                                        </div>
                                        <div class="finance-value">
                                            <asp:Label runat="server" ID="lblPriceOrder"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="detail-row">
                                        <div class="detail-label">
                                            <asp:Label runat="server" ID="lblGstTitle"></asp:Label>
                                        </div>
                                        <div class="finance-value">
                                            <asp:Label runat="server" ID="lblGst"></asp:Label>
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="detail-row">
                                        <div class="detail-label fw-bold">
                                            <asp:Label runat="server" ID="lblFinalPriceOrderTitle"></asp:Label>
                                        </div>
                                        <div class="finance-value">
                                            <asp:Label runat="server" ID="lblFinalPriceOrder"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-12">
                <div class="card border-0 shadow-sm">
                    <div class="card-header bg-white">
                        <div class="d-flex justify-content-between align-items-center flex-wrap">
                            <div>
                                <h5 class="mb-1 fw-bold">Order Items</h5>
                                <div class="text-muted small">Products and services included in this order</div>
                            </div>
                            <div class="mt-2 mt-lg-0">
                                <a href="javascript:void(0);" runat="server" id="aAddItem" class="btn btn-primary me-2" data-bs-toggle="modal" data-bs-target="#modalAddItem">New Item</a>
                                <a href="javascript:void(0);" runat="server" id="aAddService" class="btn btn-outline-secondary me-2" data-bs-toggle="modal" data-bs-target="#modalAddService">New Service</a>
                            </div>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-lg-6 mb-2" runat="server" id="divFuelSurcharge">
                                <div class="border rounded p-3 bg-light h-100">
                                    <div class="fw-semibold text-warning mb-1">Fuel Surcharge</div>
                                    <div class="small text-muted">A $4 fuel surcharge will be applied once the order is submitted.</div>
                                </div>
                            </div>
                            <div class="col-lg-6 mb-2" runat="server" id="divMinimumOrderSurcharge">
                                <div class="border rounded p-3 bg-light h-100">
                                    <div class="fw-semibold text-warning mb-1">Minimum Order Surcharge</div>
                                    <div class="small text-muted">This order will incur an additional minimum order surcharge of $15 after submission.
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="border rounded">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvListItem" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="100" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvListItem_PageIndexChanging" OnRowCommand="gvListItem_RowCommand">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                        <asp:BoundField DataField="ProductId" HeaderText="Product ID" />
                                        <asp:TemplateField HeaderText="Description">
                                            <ItemTemplate>
                                                <%# BindProductDescription(Eval("Id")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Buy Price">
                                            <ItemTemplate>
                                                <%# ItemCosting(Eval("Id").ToString(), "BuyPrice") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sell Price">
                                            <ItemTemplate>
                                                <%# ItemCosting(Eval("Id").ToString(), "SellPrice") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Price">
                                            <ItemTemplate>
                                                <%# ItemCosting(Eval("Id").ToString(), "SellPrice") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Mark Up">
                                            <ItemTemplate>
                                                <%# BindMarkUp(Eval("MarkUp")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-Width="130px" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                <ul class="dropdown-menu">
                                                    <li>
                                                        <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleCopy(Eval("ProductId").ToString()) %>'>
                                                        <asp:LinkButton runat="server" ID="linkCopy" CssClass="dropdown-item" Text="Copy" CommandName="Copy" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleDelete(Eval("ProductId").ToString()) %>'>
                                                        <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDeleteItem" onclick='<%# String.Format("return dataDeleteItem(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                    </li>
                                                    <li runat="server" visible='<%# VisiblePrinting(Eval("Id").ToString()) %>'>
                                                        <hr class="dropdown-divider" />
                                                    </li>
                                                    <li runat="server" visible='<%# VisiblePrinting(Eval("Id").ToString()) %>'>
                                                        <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkPrinting" Text="Printing Fabric" CommandName="Printing" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleCosting() %>'>
                                                        <hr class="dropdown-divider" />
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleCosting() %>'>
                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="loadCostings('<%# Eval("Id") %>', '<%= lblCompanyId.Text %>')">Costing</a>
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleEditPrice() %>'>
                                                        <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkEditCosting" Text="Edit Costing" CommandName="EditCosting" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleLog() %>'>
                                                        <hr class="dropdown-divider" />
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleLog() %>'>
                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderDetails', '<%# Eval("Id") %>')">Log</a>
                                                    </li>
                                                </ul>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                    <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-12">
                <div class="row mb-2" runat="server" id="divErrorB">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgErrorB"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Changelog</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger d-none" id="logError"></div>
                    <div class="table-responsive">
                        <table class="table table-vcenter card-table" id="tblLogs">
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalDateOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Date Order</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Created Date</label>
                            <asp:TextBox runat="server" ID="txtCreatedDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Submitted Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtSubmittedDate" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Production Date</label>
                            <asp:TextBox runat="server" ID="txtProductionDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Hold Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtHoldDate" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Completed Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtCompletedDate" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Canceled Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtCanceledDate" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Quoted Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtQuotedDate" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDateOrder" CssClass="btn btn-primary" Text="Submit" OnClick="btnDateOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalCostingBuy" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header ">
                    <h5 class="modal-title">JPMD Buy</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover mb-0">
                             <thead>
                                 <tr>
                                     <th>Total Excl. GST</th>
                                     <th>GST 10%</th>
                                     <th>Total Incl. GST</th>
                                 </tr>
                             </thead>
                             <tbody>
                                 <tr>
                                     <td><span runat="server" id="spanOrderBuy"></span></td>
                                     <td><span runat="server" id="spanGstBuy"></span></td>
                                     <td><span runat="server" id="spanTotalBuy"></span></td>
                                 </tr>
                             </tbody>
                         </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalRePrice" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Re-Price Order</h5>
                </div>
                <div class="modal-body py-4">
                    Hi <b><%: Session("FullName") %></b>,<br /><br />
                    <b>The price will be recalculated using the latest pricing version.</b><br /><br />
                    If the issue persists, please contact the IT team.<br />
                    This issue is caused by a product group that is not registered in the system.
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnRecalculate" CssClass="btn btn-info" Text="Confirm" OnClick="btnRecalculate_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalDownloadBOE" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-dark">
                    <h5 class="modal-title white">Download BOE</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />
                    Teks
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDownloadBOE" CssClass="btn btn-dark" Text="Confirm" OnClick="btnDownloadBOE_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalAddNote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add Internal Note</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Note</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtAddNote" CssClass="form-control" Height="130px" placeholder="Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorAddNote">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorAddNote"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddNote" CssClass="btn btn-dark" Text="Submit" OnClick="btnAddNote_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalHistoryNote" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">History Internal Note</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger d-none" id="historyNoteError"></div>
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover mb-0">
                            <thead>
                                <tr>
                                    <th style="width:50px"></th>
                                    <th>Created By</th>
                                    <th>Created Date</th>
                                    <th>Note</th>
                                </tr>
                            </thead>
                            <tbody id="historyNoteBody"></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer"></div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteOrder" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalDuplicateOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white">Duplicate Order</h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Number (New)</label>
                            <asp:TextBox runat="server" ID="txtOrderNumberNew" CssClass="form-control" placeholder="Order Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Name (New)</label>
                            <asp:TextBox runat="server" ID="txtOrderNameNew" CssClass="form-control" placeholder="Order Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Note (New)</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtOrderNoteNew" Height="100px" CssClass="form-control" placeholder="Order Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorDuplicateOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorDuplicateOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDuplicateOrder" CssClass="btn btn-warning" Text="Confirm" OnClick="btnDuplicateOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalQuoteOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Quote Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnQuoteOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnQuoteOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalSubmitOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Submit Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                    <br /><br />
                    <i>* Make sure you have reviewed this order before sending it to the factory.</i>
                    <br /><br />
                    <asp:CheckBox runat="server" ID="chkSendEmail" CssClass="form-control" Checked="true" Text="&nbsp;&nbsp;&nbsp;SEND EMAIL CONFIRM" />
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitOrder" CssClass="btn btn-success" Text="Confirm" OnClick="btnSubmitOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalNewOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">New Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnNewOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnNewOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalUnsubmitOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Unsubmit Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUnsubmitOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnUnsubmitOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalCancelOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Cancel Order</h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtCancelDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorCancelOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorCancelOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCancelOrder" CssClass="btn btn-info" Text="Submit" OnClick="btnCancelOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>    
    <div class="modal fade text-center" id="modalProductionOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Production Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProductionOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnProductionOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalHoldOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Hold Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnHoldOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnHoldOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalShippedOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Shipped Order</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Shipment Number</label>
                            <asp:TextBox runat="server" ID="txtShipmentNumber" CssClass="form-control" placeholder="Shipment Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Shipment Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtShipmentDate" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Container Number</label>
                            <asp:TextBox runat="server" ID="txtContainerNumber" CssClass="form-control" placeholder="Container Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Container ETA</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtContainerEta" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Courier</label>
                            <asp:TextBox runat="server" ID="txtCourier" CssClass="form-control" placeholder="Courier ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>		
		            <div class="row mb-2" runat="server" id="divErrorShippedOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorShippedOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnShippedOrder" CssClass="btn btn-info" Text="Submit" OnClick="btnShippedOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalCompleteOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Complete Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCompleteOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnCompleteOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalReworkOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Rework Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                    <br /><br />
                    After confirmation, you will be redirected to the <b>Rework Detail</b> page.
                    <br /><br />
                    Please add the item(s) that need to be reworked.
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnReworkOrder" CssClass="btn btn-danger" Text="Confirm" OnClick="btnReworkOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalDetailQuote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-full modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Quote Details</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 col-sm-12 col-lg-6">
                            <div class="row mb-1">
                                <div class="col-6 form-group">
                                    <label class="form-label">Email</label>
                                    <asp:TextBox runat="server" ID="txtQuoteEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-6 form-group">
                                    <label class="form-label">Phone</label>
                                    <asp:TextBox runat="server" ID="txtQuotePhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row mb-1">
                                <div class="col-6 form-group">
                                    <label class="form-label">Address</label>
                                    <asp:TextBox runat="server" ID="txtQuoteAddress" CssClass="form-control" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-6 form-group">
                                    <label class="form-label">Suburb</label>
                                    <asp:TextBox runat="server" ID="txtQuoteSuburb" CssClass="form-control" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row mb-1">
                                <div runat="server" id="divQuoteCity" class="col-6 form-group">
                                    <label class="form-label">City</label>
                                    <asp:TextBox runat="server" ID="txtQuoteCity" CssClass="form-control" placeholder="City ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-6 form-group">
                                    <label class="form-label">State</label>
                                    <asp:TextBox runat="server" ID="txtQuoteState" CssClass="form-control" placeholder="State ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row mb-1">
                                <div class="col-6 form-group">
                                    <label class="form-label">Post Code</label>
                                    <asp:TextBox runat="server" ID="txtQuotePostCode" CssClass="form-control" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6">
                            <div class="row mb-1">
                                <div class="col-6 form-group">
                                    <label class="form-label">Discount</label>
                                    <div class="input-group">
                                        <span runat="server" id="spanDiscount" class="input-group-text">$</span>
                                        <asp:TextBox runat="server" ID="txtQuoteDiscount" CssClass="form-control" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-6 form-group">
                                    <label class="form-label">Check Measure</label>                            
                                    <div class="input-group">
                                        <span runat="server" id="spanMeasure" class="input-group-text">$</span>
                                        <asp:TextBox runat="server" ID="txtQuoteCheckMeasure" CssClass="form-control" placeholder="Check Measure ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb-1">
                                <div class="col-6 form-group">
                                    <label class="form-label">Installation</label>
                                    <div class="input-group">
                                        <span runat="server" id="spanInstall" class="input-group-text">$</span>
                                        <asp:TextBox runat="server" ID="txtQuoteInstallation" CssClass="form-control" placeholder="Installation ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-6 form-group">
                                    <label class="form-label">Freight</label>
                                    <div class="input-group">
                                        <span runat="server" id="spanFreight" class="input-group-text">$</span>
                                        <asp:TextBox runat="server" ID="txtQuoteFreight" CssClass="form-control" placeholder="Freight ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorDetailQuote">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorDetailQuote"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer justify-content-start">
                    <asp:Button runat="server" ID="btnDetailQuote" CssClass="btn btn-info" Text="Submit" OnClick="btnDetailQuote_Click" OnClientClick="return showWaiting();" />
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDownloadQuote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Download Quote</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDownloadQuote" CssClass="btn btn-info" Text="Confirm" OnClick="btnDownloadQuote_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalSendQuote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Send Quote</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">To</label>
                            <asp:TextBox runat="server" ID="txtSendQuoteTo" CssClass="form-control" placeholder="Customer Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC Customer</label>
                            <asp:TextBox runat="server" ID="txtSendQuoteCCCustomer" TextMode="MultiLine" CssClass="form-control" Height="135px" placeholder="CC Customer ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            <p><small class="text-muted">* Please enter the required email address in a new line.</small></p>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC Staff</label>
                            <asp:TextBox runat="server" ID="txtSendQuoteCCStaff" TextMode="MultiLine" CssClass="form-control"  Height="135px" placeholder="CC ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            <p><small class="text-muted">* Please enter the required email address in a new line.</small></p>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorSendQuote">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorSendQuote"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSendQuote" CssClass="btn btn-primary" Text="Submit" OnClick="btnSendQuote_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
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
                    <div class="row mb-2" runat="server" id="divErrorSendInvoice">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorSendInvoice"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSendInvoice" CssClass="btn btn-primary" Text="Submit" OnClick="btnSendInvoice_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDownloadInvoice" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-primary">
                    <h5 class="modal-title white">Download Invoice</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDownloadInvoice" CssClass="btn btn-primary" Text="Confirm" OnClick="btnDownloadInvoice_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDownloadInvoiceCSV" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-primary">
                    <h5 class="modal-title white">Download Invoice (CSV)</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDownloadInvoiceCSV" CssClass="btn btn-primary" Text="Confirm" OnClick="btnDownloadInvoiceCSV_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalInvoiceNumber" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Invoice Number</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">New Invoice Number</label>
                            <asp:TextBox runat="server" ID="txtUpdateInvoiceNumber" CssClass="form-control" placeholder="Invoice Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorInvoiceNumber">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorInvoiceNumber"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnInvoiceNumber" CssClass="btn btn-primary" Text="Submit" OnClick="btnInvoiceNumber_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalInvoiceData" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Update Invoice Data</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Invoice Number</label>
                            <asp:TextBox runat="server" ID="txtInvoiceNumber" CssClass="form-control" placeholder="Invoice Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Collector</label>
                            <asp:DropDownList runat="server" ID="ddlCollector" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Invoice Date</label>
                            <asp:TextBox runat="server" ID="txtInvoiceDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Due Date</label>
                            <asp:TextBox runat="server" ID="txtDueDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>                        
                    </div>
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Payment Status</label>
                            <asp:DropDownList runat="server" ID="ddlPayment" CssClass="form-select">
                                <asp:ListItem Value="0" Text="Unpaid"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Paid"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Payment Date</label>
                            <asp:TextBox runat="server" ID="txtPaymentDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorInvoiceData">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorInvoiceData"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnInvoiceData" CssClass="btn btn-primary" Text="Submit" OnClick="btnInvoiceData_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalReceivePayment" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-primary">
                    <h5 class="modal-title white">Receive Payment</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnReceivePayment" CssClass="btn btn-primary" Text="Confirm" OnClick="btnReceivePayment_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalBuilderDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Builder Data</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorBuilderDetail">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorBuilderDetail"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6 form-group">
                            <label class="form-label">Estimator</label>
                            <asp:TextBox runat="server" ID="txtEstimator" CssClass="form-control" placeholder="Estimator ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Supervisor</label>
                            <asp:TextBox runat="server" ID="txtSupervisor" CssClass="form-control" placeholder="Supervisor ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Address</label>
                            <asp:TextBox runat="server" ID="txtAddress" CssClass="form-control" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6 form-group">
                            <label class="form-label">Call For Check Measure</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtCallForCheckMeasure" CssClass="form-control" placeholder="Call Up ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Check Measure Due</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtCheckMeasureDue" CssClass="form-control" placeholder="Check Measure ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6 form-group">
                            <label class="form-label">To Be Installed</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtToBeInstalled" CssClass="form-control" placeholder="Installation ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Installed</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtInstalled" CssClass="form-control" placeholder="Installed ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnBuilderDetail" CssClass="btn btn-primary" Text="Submit" OnClick="btnBuilderDetail_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalFileOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Order File</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorFileOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorFileOrder"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-12">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvListOrderFile" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" OnRowCommand="gvListOrderFile_RowCommand">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="180px">
                                            <ItemTemplate>
                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                <ul class="dropdown-menu">
                                                    <li>
                                                        <a class="dropdown-item" href='<%# ResolveUrl("~/Handler/Download.ashx?folder=" & Eval("FolderName") & "&file=" & Eval("FileName")) %>'>Download</a>
                                                    </li>
                                                    <li runat="server" visible='<%# VisibleFileDelete() %>'>
                                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="dropdown-item" CommandName="DeleteFile" CommandArgument='<%# Eval("FileName") %>' OnClientClick="return confirm('Are you sure want to delete this file?');">Delete</asp:LinkButton>
                                                    </li>
                                                </ul>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divUploadAction">
                        <div class="col-12 form-group">
                            <label class="form-label">Upload New File</label>
                            <asp:FileUpload runat="server" ID="fuOrderFile" CssClass="form-control" />
                        </div>
                        <div class="col-12">
                            <asp:Button runat="server" ID="btnUploadFileOrder" CssClass="btn btn-secondary" Text="Upload" OnClick="btnUploadFileOrder_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalConvertOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Convert Order</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Job Number</label>
                            <asp:TextBox runat="server" ID="txtConvertNumber" CssClass="form-control" placeholder="Job Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">WO Number</label>
                            <asp:TextBox runat="server" ID="txtConvertWorkNumber" CssClass="form-control" placeholder="WO Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Note</label>
                            <asp:TextBox runat="server" ID="txtConvertNote" CssClass="form-control" placeholder="Note ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorConvertOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorConvertOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnConvertOrder" CssClass="btn btn-primary" Text="Confirm" OnClick="btnConvertOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalReConvertOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Re-Convert Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnReConvertOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnReConvertOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalDataJob" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header ">
                    <h5 class="modal-title">Data Job</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover mb-0">
                             <thead>
                                 <tr>
                                     <th>Job Number</th>
                                     <th>Work Number</th>
                                     <th>Job Note</th>
                                     <th>Converted By</th>
                                     <th>Converted Date</th>
                                 </tr>
                             </thead>
                             <tbody>
                                 <tr>
                                     <td><span runat="server" id="spanJobNumber"></span></td>
                                     <td><span runat="server" id="spanWorkOrder"></span></td>
                                     <td><span runat="server" id="spanJobNote"></span></td>
                                     <td><span runat="server" id="spanJobCreatedBy"></span></td>
                                     <td><span runat="server" id="spanJobCreatedDate"></span></td>
                                 </tr>
                             </tbody>
                         </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalUpdateJobData" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Update Job Data</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Job Number</label>
                            <asp:TextBox runat="server" ID="txtJobNumber" CssClass="form-control" placeholder="Job Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">WO Number</label>
                            <asp:TextBox runat="server" ID="txtUpdateWorkNumber" CssClass="form-control" placeholder="WO Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Note</label>
                            <asp:TextBox runat="server" ID="txtUpdateNote" CssClass="form-control" placeholder="Note ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorUpdateJobData">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorUpdateJobData"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUpdateJobData" CssClass="btn btn-primary" Text="Submit" OnClick="btnUpdateJobData_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalAddItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Item</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label required">SELECT PRODUCT</label>
                            <asp:DropDownList runat="server" ID="ddlDesign" CssClass="choices form-select"></asp:DropDownList>
                            <small class="form-hint" style="color:red;">* Please select a product then click the submit button</small>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddItem" CssClass="btn btn-primary" Text="Submit" OnClick="btnAddItem_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalAddService" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Item Service</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label required">SELECT ITEM</label>
                            <asp:DropDownList runat="server" ID="ddlAddService" CssClass="choices form-select"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label required">ADDITIONAL NOTE</label>
                            <asp:TextBox runat="server" ID="txtNoteService" CssClass="form-control" placeholder="Additional Note ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label required">BUY PRICE</label>
                            <asp:TextBox runat="server" ID="txtBuyService" CssClass="form-control" placeholder="Buy Price ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label required">SELL PRICE</label>
                            <asp:TextBox runat="server" ID="txtSellService" CssClass="form-control" placeholder="Sell Price ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorAddService">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorAddService"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddServices" CssClass="btn btn-primary" Text="Submit" OnClick="btnAddService_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteItem" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Item Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtDeleteItemId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteItem" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteItem_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>    
    <div class="modal modal-blur fade" id="modalCosting" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Price Details</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger d-none" id="costingError"></div>
                    <div class="table-responsive">
                         <table class="table table-bordered table-hover mb-0">
                             <thead id="costingHead"></thead>
                             <tbody id="costingBody"></tbody>
                         </table>
                    </div>
                    <div class="modal-footer"></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalWaiting" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-body text-center py-4">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblOrderJobId"></asp:Label>
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyId"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyDetailId"></asp:Label>
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblPriceGroupId"></asp:Label>
        <asp:Label runat="server" ID="lblCreatedRole"></asp:Label>
        <asp:Label runat="server" ID="lblOrderStatusDescription"></asp:Label>
        <asp:Label runat="server" ID="lblOrderPaid"></asp:Label>
        <asp:Label runat="server" ID="lblDownloadBoe"></asp:Label>
    </div>
    
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvListItem.ClientID %>');
            if (!gv) return;
            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';
                row.addEventListener('click', function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                        return;
                    }
                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                });
            }
        });
        [
            "modalLog", "modalWaiting", "modalBuilderDetail", "modalFileOrder", "modalCostingBuy", "modalDateOrder", "modalDuplicateOrder", "modalAddNote", "modalHistoryNote", "modalRePrice", "modalDownloadBOE",
            "modalDeleteOrder", "modalQuoteOrder", "modalSubmitOrder", "modalUnsubmitOrder", "modalCancelOrder", "modalProductionOrder", "modalHoldOrder", "modalShippedOrder", "modalCompleteOrder",
            "modalReworkOrder",
            "modalSendInvoice", "modalReceivePayment", "modalDownloadInvoice", "modalDownloadInvoiceCSV", "modalInvoiceNumber", "modalInvoiceData",
            "modalDetailQuote", "modalDownloadQuote", "modalSendQuote",
            "modalConvertOrder", "modalReConvertOrder", "modalDataJob", "modalUpdateJobData",
            "modalAddItem", "modalAddService", "modalDeleteItem", "modalCosting", 
            
        ].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        function showLogFromElement(el) {
            var id = el.getAttribute("data-id");
            showLog('OrderHeaders', id);
        }
        function showLog(type, dataId) {
            $("#logError").addClass("d-none").html("");
            $("#tblLogs tbody").html("");
            $("#modalLog").modal("show");

            $.ajax({
                type: "POST",
                url: "Method.aspx/GetLogs",
                data: JSON.stringify({ type: type, dataId: dataId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    const logs = res.d;
                    if (!logs || logs.length === 0) {
                        $("#tblLogs tbody").html(
                            `<tr><td class="text-center">DATA LOG NOT FOUND</td></tr>`
                        );
                        return;
                    }
                    let html = "";
                    logs.forEach(r => {
                        html += `<tr><td>${r.TextLog}</td></tr>`;
                    });
                    $("#tblLogs tbody").html(html);
                },
                error: function (err) {
                    $("#logError").removeClass("d-none").html("FAILED TO LOAD LOG DATA");
                }
            });
        }
        function loadCostings(itemId, companyId) {
            $("#costingError").addClass("d-none").html("");
            $("#costingHead").html("");
            $("#costingBody").html("");
            $("#modalCosting").modal("show");

            $.ajax({
                type: "POST",
                url: "Method.aspx/GetCostings",
                data: JSON.stringify({ itemId, companyId }),
                contentType: "application/json",
                dataType: "json",
                success: res => {
                    const perm = {
                        showType: res.d.showType,
                        showBuy: res.d.showBuy,
                        showSell: res.d.showSell,
                        showPrice: res.d.showPrice
                    };

                    renderCostingTable(res.d.data, perm);
                },
                error: () => {
                    $("#costingError").removeClass("d-none").text("FAILED TO LOAD COSTING DATA");
                }
            });
        }
        function renderCostingTable(data, perm) {
            let headHtml = "<tr>";
            if (perm.showType) headHtml += "<th>Type</th>";
            headHtml += "<th>Description</th>";
            if (perm.showBuy) headHtml += "<th>Buy Price</th>";
            if (perm.showSell) headHtml += "<th>Sell Price</th>";
            if (perm.showPrice) headHtml += "<th>Price</th>";
            headHtml += "</tr>";

            $("#costingHead").html(headHtml);

            const colCount = $("#costingHead th").length;

            if (!data || data.length === 0) {
                $("#costingBody").html(
                    `<tr><td colspan="${colCount}" class="text-center">
                DATA NOT FOUND :)
             </td></tr>`
                );
                return;
            }

            let bodyHtml = "";
            data.forEach(r => {
                bodyHtml += "<tr>";
                if (perm.showType) bodyHtml += `<td>${r.Type}</td>`;
                bodyHtml += `<td>${r.Description}</td>`;
                if (perm.showBuy) bodyHtml += `<td>${r.BuyPricing}</td>`;
                if (perm.showSell) bodyHtml += `<td>${r.SellPricing}</td>`;
                if (perm.showPrice) bodyHtml += `<td>${r.Price}</td>`;
                bodyHtml += "</tr>";
            });

            $("#costingBody").html(bodyHtml);
        }
        function showBuilderDetail() {
            $("#modalBuilderDetail").modal("show");
        }
        function showFileOrder() {
            $("#modalFileOrder").modal("show");
        }
        function showAddNote() {
            $("#modalAddNote").modal("show");
        }
        function showHistoryNote(headerId) {
            $("#historyNoteError").addClass("d-none").html("");
            $("#historyNoteBody").html("");
            $("#modalHistoryNote").modal("show");

            $.ajax({
                type: "POST",
                url: "Method.aspx/GetHistoryNote",
                data: JSON.stringify({ headerId }),
                contentType: "application/json",
                dataType: "json",
                success: res => {
                    const data = res.d;

                    if (!data || data.length === 0) {
                        $("#historyNoteBody").html(
                            `<tr><td colspan="4" class="text-center">DATA NOT FOUND :)</td></tr>`
                        );
                        return;
                    }

                    let html = "";
                    data.forEach((r, i) => {
                        html += `<tr>
                            <td class="text-center">${i + 1}</td>
                            <td>${r.FullName}</td>
                            <td>${r.CreatedDate}</td>
                            <td>${r.Note}</td>
                        </tr>`;
                    });

                    $("#historyNoteBody").html(html);
                },
                error: () => {
                    $("#historyNoteError").removeClass("d-none").text("FAILED TO LOAD HISTORY NOTE");
                }
            });
        }
        function showService() {
            $("#modalAddService").modal("show");
        }
        function showDetailQuote() {
            $("#modalDetailQuote").modal("show");
        }
        function showSendQuote() {
            $("#modalSendQuote").modal("show");
        }
        function showSendInvoice() {
            $("#modalSendInvoice").modal("show");
        }
        function showInvoiceNumber() {
            $("#modalInvoiceNumber").modal("show");
        }
        function showInvoiceData() {
            $("#modalInvoiceData").modal("show");
        }
        function showDuplicateOrder() {
            $("#modalDuplicateOrder").modal("show");
        }
        function showCancelOrder() {
            $("#modalCancelOrder").modal("show");
        }
        function showShippedOrder() {
            $("#modalShippedOrder").modal("show");
        }
        function showReworkOrder() {
            $("#modalReworkOrder").modal("show");
        }
        function showCostingBuy() {
            $("#modalCostingBuy").modal("show");
        }
        function showConvertOrder() {
            $("#modalConvertOrder").modal("show");
        }
        function showUpdateJobData() {
            $("#modalUpdateJobData").modal("show");
        }
        function showDateOrder() {
            $("#modalDateOrder").modal("show");
        }
        function dataDeleteItem(id) {
            document.getElementById("<%=txtDeleteItemId.ClientID %>").value = id;
        }
        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 1000);

            return true;
        }
        function hideWaitingModal() {
            var modalEl = document.getElementById('modalWaiting');
            if (modalEl) {
                var modalObj = bootstrap.Modal.getInstance(modalEl);
                if (modalObj) modalObj.hide();
            }
        }
        function showSuccessSwal(orderId) {
            hideWaitingModal();
            Swal.fire({
                icon: 'success',
                title: 'Order Submitted!',
                html: 'Your order has been successfully submitted and is now being processed.<br>Thank you!',
                timer: 3000,
                timerProgressBar: true,
                didClose: () => {
                    window.location = '/order/detail?orderid=' + orderId;
                }
            });
        }
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>