<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Order_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Order Detail" %>

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

        <section class="row mb-4">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <a href="javascript:void(0);" class="btn btn-secondary me-1" onclick="showLog('OrderHeaders', '<%= lblHeaderId.Text %>')">Log</a>
                <asp:Button runat="server" ID="btnPreview" CssClass="btn btn-primary me-1" Text="Preview" />
                <asp:Button runat="server" ID="btnEditHeader" CssClass="btn btn-secondary me-1" Text="Edit Header" OnClick="btnEditHeader_Click" />
                <a href="#" runat="server" id="aDeleteOrder" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalDeleteOrder">Delete Order</a>
                <a href="#" runat="server" id="aQuoteOrder" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalQuoteOrder">Quote</a>
                <a href="#" runat="server" id="aSubmitOrder" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalSubmitOrder">Submit Order</a>
                <a href="#" runat="server" id="aUnsubmitOrder" class="btn btn-dark me-1" data-bs-toggle="modal" data-bs-target="#modalUnsubmitOrder">Unsubmit Order</a>
                <a href="#" runat="server" id="aCancelOrder" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalCancelOrder">Cancel Order</a>
                <a href="#" runat="server" id="aProductionOrder" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalProductionOrder">Production Order</a>
                <a href="#" runat="server" id="aHoldOrder" class="btn btn-warning me-1" data-bs-toggle="modal" data-bs-target="#modalHoldOrder">Hold Order</a>
                <a href="#" runat="server" id="aUnHoldOrder" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalUnHoldOrder">Production Order</a>
                <a href="#" runat="server" id="aShippedOrder" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalShippedOrder">Shipped Order</a>
                <a href="#" runat="server" id="aCompleteOrder" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalCompleteOrder">Complete Order</a>
                <a href="#" runat="server" id="aReworkOrder" class="btn btn-warning me-1" data-bs-toggle="modal" data-bs-target="#modalReworkOrder">Rework Order</a>
                
                <button class="btn btn-info dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnQuoteAction">Quote</button>
                <ul class="dropdown-menu">
                    <li>
                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDetailQuote">Quote Details</a>
                    </li>
                    <li>
                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDownloadQuote">Download Quote</a>
                    </li>
                </ul>

                <button class="btn btn-primary dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnInvoice">invoice</button>
                <ul class="dropdown-menu">
                    <li>
                        <a href="#" runat="server" id="aSendInvoice" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalSendInvoice">Send Invoice</a>
                    </li>
                    <li>
                        <a href="#" runat="server" id="aReceivePayment" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalReceivePayment">Receive Payment</a>
                    </li>
                    <li>
                        <a href="#" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDownloadInvoice">Download Invoice</a>
                    </li>
                    <li runat="server" id="liDividerInvoice"><hr class="dropdown-divider"></li>
                    <li runat="server" id="liUpdateInvoiceNumber">
                        <a href="#" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalInvoiceNumber">Update Invoice Number</a>
                    </li>
                    <li runat="server" id="liUpdateInvoiceData">
                        <a href="#" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalInvoiceData">Update Invoice Data</a>
                    </li>
                </ul>

                <a href="#" runat="server" id="aBuilder" class="btn btn-warning me-1" data-bs-toggle="modal" data-bs-target="#modalBuilderDetail">Builder Details</a>

                <a href="#" runat="server" id="aFileOrder" class="btn btn-secondary me-1" data-bs-toggle="modal" data-bs-target="#modalFileOrder">Files</a>

                <button class="btn btn-dark dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnMoreAction">More</button>
                <ul class="dropdown-menu">
                    <li runat="server" id="liMoreDownloadQuote">
                        <a href="#" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalMoreDownloadQuote">Download Quote</a>
                    </li>
                    <li runat="server" id="liMoreEmailQuote">
                        <a href="#" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalMoreEmailQuote">Email Quote</a>
                    </li>
                    <li runat="server" id="liMoreDividerQuote"><hr class="dropdown-divider"></li>
                    <li runat="server" id="liMoreAddNote">
                        <a href="#" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalAddNote">Add Internal Note</a>
                    </li>                    
                    <li runat="server" id="liMoreHistoryNote">
                        <a href="javascript:void(0)" class="dropdown-item" onclick="showHistoryNote('<%= lblHeaderId.Text %>')">History Note</a>
                    </li>
                </ul>
            </div>
        </section>

        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-2">
                                <div class="col-12 col-sm-12 col-lg-9 mb-2">
                                    <label>Customer Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCustomerName" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                    <label>Order #</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderId" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-2">
                                <div class="col-6 col-sm-6 col-lg-6 mb-2">
                                    <label>Customer Order Number</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderNumber" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-6 mb-2">
                                    <label>Customer Order Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderName" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-2">
                                <div class="col-6 col-sm-6 col-lg-5 mb-2">
                                    <label>Order Status</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderStatus" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-7 mb-2">
                                    <label>Order Type</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderType" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-2">
                                <div class="col-12 col-sm-6 col-lg-7 mb-2">
                                    <label>Order Note</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderNote" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-4">
                                    <label>CreatedBy</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCreatedBy" Visible="false"></asp:Label>
                                    <asp:Label runat="server" ID="lblCreatedName" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-8" runat="server" id="divInternalNote">
                                    <label>Internal Note (Latest)</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblInternalNote" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-12 col-sm-12 col-lg-5">
                <div class="row">
                    <div class="col-12">
                        <div class="card">
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="row mb-1">
                                        <div class="col-6 col-sm-6 col-lg-6">
                                            <label>Created Date</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblCreatedDate" CssClass="font-bold"></asp:Label>
                                        </div>
                                        <div class="col-6 col-sm-6 col-lg-6">
                                            <label>Submitted Date</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblSubmittedDate" CssClass="font-bold"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row mb-1">
                                        <div class="col-6 col-sm-6 col-lg-6">
                                            <label>Production Date</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblProductionDate" CssClass="font-bold"></asp:Label>
                                        </div>
                                        <div class="col-6 col-sm-6 col-lg-6">
                                            <label>On Hold Date</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblOnHoldDate" CssClass="font-bold"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="row mb-1">
                                        <div class="col-6 col-sm-6 col-lg-6">
                                            <label>Canceled Date</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblCanceledDate" CssClass="font-bold"></asp:Label>
                                        </div>
                                        <div class="col-6 col-sm-6 col-lg-6">
                                            <label>Completed Date</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblCompletedDate" CssClass="font-bold"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-12">
                        <div class="card" runat="server" id="divPricing">
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="row mb-3">
                                        <div class="col-6 col-sm-6 col-lg-4">
                                            <asp:Label runat="server" ID="lblPriceOrderTitle"></asp:Label>
                                            <br />
                                            <asp:Label runat="server" ID="lblPriceOrder" CssClass="font-bold"></asp:Label>
                                        </div>
                                        <div class="col-6 col-sm-6 col-lg-4">
                                            <asp:Label runat="server" ID="lblGstTitle"></asp:Label>
                                            <br />
                                            <asp:Label runat="server" ID="lblGst" CssClass="font-bold"></asp:Label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4">
                                            <asp:Label runat="server" ID="lblFinalPriceOrderTitle"></asp:Label>
                                            <br />
                                            <asp:Label runat="server" ID="lblFinalPriceOrder" CssClass="font-bold"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section class="row" runat="server" id="secBuilder">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-2">
                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>Estimator</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblEstimator" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>Supervisor</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblSupervisor" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>Address</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblAddress" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>Quoted Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblQuotedDate" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>Call For Check Measure</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCallForCheckMeasure" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>Check Measure Due</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCheckMeasureDue" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                            
                            <div class="row mb-1">
                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>To Be Installed</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblToBeInstalled" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-2 col-sm-2 col-lg-2">
                                    <label>Installed</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblInstalled" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section class="row">
            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Invoiced Number</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblInvoiceNumber" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Invoiced Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblInvoiceDate" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Collector</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCollector" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Payment Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblPaymentDate" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Shipment No</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblShipmentNumber" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Shipment Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblShipmentDate" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Container No</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblContainerNumber" CssClass="font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-3">
                                    <label>Courier</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCourier" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        
        <section class="row mb-3">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-5">
                                    <h3 class="card-title">Order Item</h3>
                                </div>

                                <div class="col-12 col-sm-12 col-lg-7 d-flex justify-content-end">
                                    <a href="#" runat="server" id="aAddItem" class="btn btn-primary me-1" data-bs-toggle="modal" data-bs-target="#modalAddItem">New Item</a>
                                    <a href="#" runat="server" id="aService" class="btn btn-info me-1" data-bs-toggle="modal" data-bs-target="#modalService">New Service</a>
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="row" runat="server" id="divMinimumOrderSurcharge">
                                <div class="col-12">
                                    <div class="alert alert-light-warning color-warning">
                                        <i class="bi bi-exclamation-circle"></i>
                                        Please note that this order will incur an additional charge: a minimum order surcharge of $15, which will be applied after the order has been submitted.
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvListItem" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvListItem_PageIndexChanging" OnRowCommand="gvListItem_RowCommand">
                                            <RowStyle />
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
                                                                <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalDeleteItem" onclick='<%# String.Format("return showDeleteItem(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                            </li>
                                                            <li runat="server" visible='<%# VisiblePrinting(Eval("Id").ToString()) %>'>
                                                                <hr class="dropdown-divider">
                                                            </li>
                                                            <li runat="server" visible='<%# VisiblePrinting(Eval("Id").ToString()) %>'>
                                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkPrinting" Text="Printing Fabric" CommandName="Printing" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# VisibleCosting() %>'><hr class="dropdown-divider"></li>
                                                            <li runat="server" visible='<%# VisibleCosting() %>'>
                                                                <a href="javascript:void(0)" class="dropdown-item" onclick="loadCostings('<%# Eval("Id") %>', '<%= lblCompanyId.Text %>')">Costing</a>
                                                            </li>
                                                            <li runat="server" visible='<%# VisibleEditPrice() %>'>
                                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkEditCosting" Text="Edit Costing" CommandName="EditCosting" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <hr class="dropdown-divider">
                                                            </li>
                                                            <li>
                                                                <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('OrderDetails', '<%# Eval("Id") %>')">Log</a>
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
                    <div class="card-footer d-flex justify-content-between"></div>
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

    <div class="modal modal-blur fade" id="modalPreview" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-full modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Preview Order</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
        
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorPreview">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorPreview"></span>
                            </div>
                        </div>
                    </div>
                    <iframe id="framePreview" runat="server" width="100%" height="600px" style="border: none;"></iframe>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteOrder" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal fade text-center" id="modalQuoteOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Quote Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnQuoteOrder" CssClass="btn btn-success" Text="Confirm" OnClick="btnQuoteOrder_Click" OnClientClick="return showWaiting();" />
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
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitOrder" CssClass="btn btn-success" Text="Confirm" OnClick="btnSubmitOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalUnsubmitOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-dark">
                    <h5 class="modal-title white">Unsubmit Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUnsubmitOrder" CssClass="btn btn-dark" Text="Confirm" OnClick="btnUnsubmitOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalCancelOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCancelOrder" CssClass="btn btn-danger" Text="Submit" OnClick="btnCancelOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal fade text-center" id="modalProductionOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Generate Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProductionOrder" CssClass="btn btn-success" Text="Confirm" OnClick="btnProductionOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalHoldOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white">Hold Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnHoldOrder" CssClass="btn btn-warning" Text="Confirm" OnClick="btnHoldOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalUnHoldOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Production Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUnHoldOrder" CssClass="btn btn-success" Text="Confirm" OnClick="btnUnHoldOrder_Click" OnClientClick="return showWaiting();" />
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
                        <div class="col-12 form-group">
                            <label class="form-label">Shipment Number</label>
                            <asp:TextBox runat="server" ID="txtShipmentNumber" CssClass="form-control" placeholder="Shipment Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Shipment Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtShipmentDate" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Container Number</label>
                            <asp:TextBox runat="server" ID="txtContainerNumber" CssClass="form-control" placeholder="Container Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Courier</label>
                            <asp:TextBox runat="server" ID="txtCourier" CssClass="form-control" placeholder="Courier ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group"></div>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnShippedOrder" CssClass="btn btn-primary" Text="Submit" OnClick="btnShippedOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalCompleteOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Complete Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCompleteOrder" CssClass="btn btn-danger" Text="Confirm" OnClick="btnCompleteOrder_Click" OnClientClick="return showWaiting();" />
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
                                    <RowStyle />
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="130px">
                                            <ItemTemplate>
                                                <a class="btn btn-sm btn-info" href='<%# ResolveUrl("~/Handler/Download.ashx?folder=" & Eval("FolderName") & "&file=" & Eval("FileName")) %>'>Download</a>
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

    <div class="modal fade" id="modalReworkOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Rework Order</h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorReworkOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorReworkOrder"></span>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListItemRework" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" DataKeyNames="Id">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                       <asp:CheckBox ID="chkSelectAll" runat="server" CssClass="form-check" onclick="toggleSelectAll(this)" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" runat="server" CssClass="form-check chkSelectItem" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <%# BindProductDescription(Eval("Id").ToString()) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </div>

                    <div class="row mt-2" runat="server" id="divReworkNote" visible="false">
                        <div class="col-12">
                            <div class="alert alert-primary">
                                You have previously created a rework request that has not yet been submitted.
                                <br />
                                The items shown are those that you have not added yet.
                                <br />
                                If you add the items above, they will be included in your existing rework request.
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer ">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnReworkOrder" CssClass="btn btn-danger" Text="Submit" OnClick="btnReworkOrder_Click" OnClientClick="return showWaiting();" />
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
                            <div class="divider">
                                <div class="divider-text">Section 1</div>
                            </div>
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
                                <div class="col-6 form-group">
                                    <label class="form-label">Country</label>
                                    <asp:DropDownList runat="server" ID="ddlQuoteCountry" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="Australia" Text="Australia"></asp:ListItem>
                                        <asp:ListItem Value="Indonesia" Text="Indonesia"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="divider"><hr /></div>

                            <div class="divider">
                                <div class="divider-text">Section 2</div>
                            </div>

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

                            <div class="divider"><hr /></div>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6">
                            <asp:Image runat="server" id="imgDetailQuote" CssClass="w-100" />
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
                    <asp:Button runat="server" ID="btnDetailQuote" CssClass="btn btn-primary" Text="Submit" OnClick="btnDetailQuote_Click" OnClientClick="return showWaiting();" />
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDownloadQuote" CssClass="btn btn-info" Text="Confirm" OnClick="btnDownloadQuote_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
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
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC Staff</label>
                            <asp:TextBox runat="server" ID="txtSendInvoiceCCStaff" CssClass="form-control" placeholder="CC ..." autocomplete="off"></asp:TextBox>
                            <p><small class="text-muted">* By default, all Account team members, Matt, and the relevant Operators will be CC’d by the system.</small></p>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDownloadInvoice" CssClass="btn btn-primary" Text="Confirm" OnClick="btnDownloadInvoice_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                            <label class="form-label">Payment Date</label>
                            <asp:TextBox runat="server" ID="txtPaymentDate" TextMode="Date" CssClass="form-control"></asp:TextBox>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnReceivePayment" CssClass="btn btn-primary" Text="Confirm" OnClick="btnReceivePayment_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalBuilderDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Builder Detail</h5>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnBuilderDetail" CssClass="btn btn-primary" Text="Submit" OnClick="btnBuilderDetail_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalMoreDownloadQuote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Download Quote</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnMoreDownloadQuote" CssClass="btn btn-info" Text="Confirm" OnClick="btnMoreDownloadQuote_Click"  OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalMoreEmailQuote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Email Quote</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">To</label>
                            <asp:TextBox runat="server" ID="txtEmailQuoteTo" CssClass="form-control" placeholder="Customer Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC Customer</label>
                            <asp:TextBox runat="server" ID="txtEmailQuoteCCCustomer" TextMode="MultiLine" CssClass="form-control" Height="135px" placeholder="CC Customer ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC Staff</label>
                            <asp:TextBox runat="server" ID="txtEmailQuoteCCStaff" CssClass="form-control" placeholder="CC Staff ..." autocomplete="off"></asp:TextBox>
                            <p><small class="text-muted">* By default, all Account team members, Matt, and the relevant Operators will be CC’d by the system.</small></p>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorMoreEmailQuote">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorMoreEmailQuote"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnMoreEmailQuote" CssClass="btn btn-primary" Text="Submit" OnClick="btnMoreEmailQuote_Click" OnClientClick="return showWaiting();" />
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddNote" CssClass="btn btn-primary" Text="Submit" OnClick="btnAddNote_Click" OnClientClick="return showWaiting();" />
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
    
    <div class="modal modal-blur fade" id="modalAddItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Item</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
            
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">SELECT PRODUCT</label>
                            <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select"></asp:DropDownList>
                            <small class="form-hint" style="color:red;">* Please select a product then click the submit button</small>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddItem" CssClass="btn btn-primary" Text="Submit" OnClick="btnAddItem_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalService" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Item</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
            
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">SELECT SERVICE</label>
                            <asp:DropDownList runat="server" ID="ddlBlindService" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorService">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorService"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnService" CssClass="btn btn-primary" Text="Submit" OnClick="btnService_Click" />
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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

    <div class="modal modal-blur fade" id="modalEditCosting" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Costing</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="card-body border-bottom py-3" runat="server" id="divErrorEditCosting">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgErrorEditCosting"></span>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:Label runat="server" ID="lblItemIdCosting" Visible="false"></asp:Label>
                        <asp:GridView runat="server" ID="gvListEditCosting" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" DataKeyNames="Id,Type">
                            <RowStyle />
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                <asp:BoundField DataField="Type" HeaderText="Type" />
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:TextBox runat="server" ID="txtDescriptionPrice" CssClass="form-control" Text='<%# Eval("Description") %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="New Price (Sell)" ItemStyle-Width="150px">
                                    <ItemTemplate>
                                        <div class="input-group">
                                            <span runat="server" id="spanEditSellPrice" class="input-group-text">$</span>
                                            <asp:TextBox runat="server" ID="txtNewSellPrice" CssClass="form-control" Text='<%# String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##}", Eval("SellPrice")) %>'></asp:TextBox>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnEditCosting" CssClass="btn btn-primary" Text="Submit" OnClick="btnEditCosting_Click" OnClientClick="return showWaiting();" />
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
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyId"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyDetailId"></asp:Label>
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblPriceGroupId"></asp:Label>
        <asp:Label runat="server" ID="lblCreatedRole"></asp:Label>
        <asp:Label runat="server" ID="lblOrderStatusDescription"></asp:Label>
        <asp:Label runat="server" ID="lblOrderPaid"></asp:Label>
    </div>
    
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvListItem.ClientID %>');
            if (!gv) return;

            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';

                row.addEventListener('click', function (e) {
                    if (
                        e.target.closest("a") ||
                        e.target.closest("button") ||
                        e.target.closest("[data-bs-toggle]")
                    ) {
                        return;
                    }

                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                });
            }
        });
        [
            "modalLog", "modalPreview", "modalWaiting", "modalBuilderDetail", "modalFileOrder",
            "modalDeleteOrder", "modalQuoteOrder", "modalSubmitOrder", "modalUnsubmitOrder", "modalCancelOrder", "modalProductionOrder", "modalHoldOrder", "modalUnHoldOrder", "modalShippedOrder", "modalCompleteOrder",
            "modalReworkOrder",
            "modalSendInvoice", "modalReceivePayment", "modalDownloadInvoice", "modalInvoiceNumber", "modalInvoiceData",
            "modalDetailQuote", "modalDownloadQuote",
            "modalMoreDownloadQuote", "modalMoreEmailQuote",
            "modalAddNote", "modalHistoryNote",
            "modalAddItem", "modalDeleteItem", "modalCosting", "modalEditCosting"
        ].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        function toggleSelectAll(source) {
            var gv = document.getElementById("<%= gvListItemRework.ClientID %>");
            var checkBoxes = gv.querySelectorAll("input[type='checkbox'][id*='chkSelect']");
            checkBoxes.forEach(function (cb) {
                cb.checked = source.checked;
            });
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

        function showPreview() {
            $("#modalPreview").modal("show");
        }

        function showBuilderDetail() {
            $("#modalBuilderDetail").modal("show");
        }

        function showFileOrder() {
            $("#modalFileOrder").modal("show");
        }

        function showDetailQuote() {
            $("#modalDetailQuote").modal("show");
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
                    $("#historyNoteError")
                        .removeClass("d-none")
                        .text("FAILED TO LOAD HISTORY NOTE");
                }
            });
        }

        function showEmailQuote() {
            $("#modalMoreEmailQuote").modal("show");
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

        function showCancelOrder() {
            $("#modalCancelOrder").modal("show");
        }

        function showShippedOrder() {
            $("#modalShippedOrder").modal("show");
        }

        function showReworkOrder() {
            $("#modalReworkOrder").modal("show");
        }

        function showEditCosting() {
            $("#modalEditCosting").modal("show");
        }

        function showService() {
            $("#modalService").modal("show");
        }

        function showDeleteItem(id) {
            document.getElementById("<%=txtDeleteItemId.ClientID %>").value = id;
        }

        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 3000);

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
                timer: 5000,
                timerProgressBar: true,
                didClose: () => {
                    window.location = '/order/detail?orderid=' + orderId;
                }
            });
        }

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>