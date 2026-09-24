<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Order_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="List Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        #divTabList .list-group-item { font-size: 14px; }
        .order-filter-header { background: #fff; border-bottom: 1px solid #e9ecef; padding: 1.25rem 1.25rem 1rem !important; }
        .order-filter-header .input-group { height: 42px; }
        .order-filter-header .input-group-text { background: #f8f9fa; border-color: #dee2e6; color: #495057; font-weight: 600; font-size: 0.9rem; min-width: 90px; justify-content: center; }
        .order-filter-header .form-select,
        .order-filter-header .form-control { border-color: #dee2e6; font-size: 0.9rem; box-shadow: none; }
        .order-filter-header .form-select:focus,
        .order-filter-header .form-control:focus { border-color: #86b7fe; box-shadow: 0 0 0 0.15rem rgba(13, 110, 253, 0.10); }
        .order-filter-header .btn-primary { min-width: 90px; font-weight: 500; }
        
        .order-grid { font-size: 0.95rem; }
        .order-grid thead th { background: #f8f9fa; color: #495057; font-size: 0.85rem; font-weight: 600; white-space: nowrap; vertical-align: middle; border-bottom: 1px solid #dee2e6; padding: 0.7rem 0.65rem; }
        .order-grid tbody td { padding: 0.65rem 0.65rem; vertical-align: middle; }
        .order-grid tbody tr { transition: background-color 0.15s ease; }
        .order-grid tbody tr:hover { background-color: rgba(13, 110, 253, 0.035); }
        .order-grid .btn-sm { font-size: 0.78rem; padding: 0.3rem 0.65rem; }
        .order-grid .dropdown-menu { font-size: 0.85rem; box-shadow: 0 0.35rem 1rem rgba(0, 0, 0, 0.10); border: 1px solid #e9ecef; }
        .order-grid .dropdown-item { padding: 0.45rem 0.85rem; }
        .order-grid-body { padding: 1rem 1.25rem 1.25rem !important; }
        .order-filter-footer { background: #fff; border-top: 1px solid #e9ecef; padding: 0.85rem 1.25rem !important; }
        .order-filter-footer .form-select { min-width: 130px; font-size: 0.85rem; border-color: #dee2e6; }
        #navPager .pagination { gap: 3px; }
        #navPager .page-link { border-radius: 6px !important; border: 1px solid #dee2e6; font-size: 0.82rem; min-width: 34px; text-align: center; }
        #navPager .page-item.active .page-link { font-weight: 600; }
        @media (max-width: 991.98px) {
            .order-filter-header { padding: 1rem !important; }
            .order-filter-header .input-group { height: 40px; }
            .order-grid-body { padding: 0.75rem !important; }
            .order-filter-footer { padding: 0.75rem 1rem !important; }
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
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row mb-3" runat="server" id="divError">
            <div class="col-12">
                <div class="row mb-2">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-12 d-flex justify-content-end flex-wrap gap-2">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Create Order" OnClick="btnAdd_Click" />
                <button class="btn btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnAddOrder">Create Order</button>
                <ul class="dropdown-menu">
                    <asp:Button runat="server" ID="btnInsert" CssClass="dropdown-item" Text="Insert Order" OnClick="btnInsert_Click" />
                    <asp:Button runat="server" ID="btnUpload" CssClass="dropdown-item" Text="Upload Order" OnClick="btnUpload_Click" />
                </ul>
                <asp:Button runat="server" ID="btnRework" CssClass="btn btn-danger" Text="Rework Order" OnClick="btnRework_Click" />
                <asp:Button runat="server" ID="btnFile" CssClass="btn btn-secondary" Text="File Order" OnClick="btnFile_Click" />
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header order-filter-header">
                                <div class="row g-2">
                                    <div class="col-12 col-lg-3" >
                                        <div class="input-group" runat="server" id="divCompany">
                                            <span class="input-group-text">Company</span>
                                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-12 col-lg-3">
                                        <div class="input-group" runat="server" id="divState">
                                            <span class="input-group-text">State</span>
                                            <asp:DropDownList runat="server" ID="ddlState" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text="All"></asp:ListItem>
                                                <asp:ListItem Value="ACT" Text="ACT"></asp:ListItem>
                                                <asp:ListItem Value="NSW" Text="NSW"></asp:ListItem>
                                                <asp:ListItem Value="NT" Text="NT"></asp:ListItem>
                                                <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                                                <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                                                <asp:ListItem Value="TAS" Text="TAS"></asp:ListItem>
                                                <asp:ListItem Value="VIC" Text="VIC"></asp:ListItem>
                                                <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-12 col-lg-6">
                                        <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                            <div class="input-group">
                                                <span class="input-group-text">Search</span>
                                                <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <div class="col-12 col-lg-3">
                                        <div class="input-group" runat="server" id="divType">
                                            <span class="input-group-text">Order Type</span>
                                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body order-grid-body">
                                <div class="list-group list-group-horizontal-sm mb-4 text-center" id="divTabList" role="tablist">
                                    <a class="list-group-item list-group-item-action active" id="listUnsubmit" data-bs-toggle="list" href="#list-unsubmit" role="tab" aria-controls="list-unsubmit">Unsubmit</a>

                                    <a class="list-group-item list-group-item-action" runat="server" id="listQuote" data-bs-toggle="list" href="#list-quote" role="tab" aria-controls="list-quote">Quote</a>

                                    <a class="list-group-item list-group-item-action" id="listWaiting" data-bs-toggle="list" href="#list-waiting" role="tab" aria-controls="list-waiting">Waiting PI</a>

                                    <a class="list-group-item list-group-item-action" id="listSent" data-bs-toggle="list" href="#list-sent" role="tab" aria-controls="list-sent">PI Sent</a>

                                    <a class="list-group-item list-group-item-action" id="listReceive" data-bs-toggle="list" href="#list-receive" role="tab" aria-controls="list-receive">Paid</a>

                                    <a class="list-group-item list-group-item-action" id="listNew" data-bs-toggle="list" href="#list-new" role="tab" aria-controls="list-new">New Order</a>

                                    <a class="list-group-item list-group-item-action" id="listProduction" data-bs-toggle="list" href="#list-production" role="tab" aria-controls="list-production">Production</a>

                                    <a class="list-group-item list-group-item-action" id="listHold" data-bs-toggle="list" href="#list-hold" role="tab" aria-controls="list-hold">Hold</a>

                                    <a class="list-group-item list-group-item-action" id="listShipped" data-bs-toggle="list" href="#list-shipped" role="tab" aria-controls="list-shipped">Shipped</a>

                                    <a class="list-group-item list-group-item-action" id="listCancel" data-bs-toggle="list" href="#list-cancel" role="tab" aria-controls="list-cancel">Canceled</a>

                                    <a class="list-group-item list-group-item-action" id="listUnshipment" data-bs-toggle="list" href="#list-unshipment" role="tab" aria-controls="list-unshipment">Unshipment</a>
                                </div>
                                <div class="tab-content text-justify">
                                    <div class="tab-pane fade show active" id="list-unsubmit" role="tabpanel" aria-labelledby="listUnsubmit">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListUnsubmit" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" AllowPaging="true" PagerSettings-Visible="false" PageSize="50" OnPageIndexChanging="gvListUnsubmit_PageIndexChanging" OnDataBound="gvListUnsubmit_DataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailUnsubmit" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                                <div class="d-flex justify-content-end mt-2">
                                                    <nav id="navUnsubmit" runat="server" visible="false">
                                                        <ul class="pagination pagination mb-0">
                                                            <asp:Repeater ID="rptUnsubmit" runat="server" OnItemCommand="rptUnsubmit_ItemCommand">
                                                                <ItemTemplate>
                                                                    <li class='page-item <%# Eval("CssClass") %>'>
                                                                        <asp:LinkButton runat="server" CssClass="page-link" Text='<%# Eval("Text") %>' CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>' />
                                                                    </li>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </ul>
                                                    </nav>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-quote" role="tabpanel" aria-labelledby="listQuote">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListQuote" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" AllowPaging="true" PagerSettings-Visible="false" PageSize="50" OnPageIndexChanging="gvListQuote_PageIndexChanging" OnDataBound="gvListQuote_DataBound">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="QuotedDate" HeaderText="Quoted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailQuote" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                                <div class="d-flex justify-content-end mt-2">
                                                    <nav id="navQuote" runat="server" visible="false">
                                                        <ul class="pagination pagination mb-0">
                                                            <asp:Repeater ID="rptQuote" runat="server" OnItemCommand="rptQuote_ItemCommand"
                                                                <ItemTemplate>
                                                                    <li class='page-item <%# Eval("CssClass") %>'>
                                                                        <asp:LinkButton runat="server" CssClass="page-link" Text='<%# Eval("Text") %>' CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>' />
                                                                    </li>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </ul>
                                                    </nav>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-waiting" role="tabpanel" aria-labelledby="listWaiting">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListWaiting" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailWaiting" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-sent" role="tabpanel" aria-labelledby="listSent">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListSent" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="InvoiceDate" HeaderText="Invoiced" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailSent" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-receive" role="tabpanel" aria-labelledby="listReceive">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListReceive" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="InvoiceDate" HeaderText="Invoiced" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="PaymentDate" HeaderText="Paid" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailReceive" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-new" role="tabpanel" aria-labelledby="listNew">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListNew" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailNew" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-production" role="tabpanel" aria-labelledby="listProduction">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListProduction" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="ProductionDate" HeaderText="Production" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailProduction" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-hold" role="tabpanel" aria-labelledby="listHold">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListHold" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="ProductionDate" HeaderText="Production" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OnHoldDate" HeaderText="Hold" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailHold" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-shipped" role="tabpanel" aria-labelledby="listShipped">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListShipped" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="ProductionDate" HeaderText="Production" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Shipment">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-info" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipment" onclick='<%# String.Format("return dataShipment(`{0}`, `{1:dd MMM yyyy}`, `{2}`, `{3:dd MMM yyyy}`, `{4}`);", Eval("ShipmentNumber").ToString(), Eval("ShipmentDate"), Eval("ContainerNumber").ToString(), Eval("ContainerETA"), Eval("Courier").ToString()) %>'>Show
                                                                    </a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                                <ItemTemplate>
                                                                    <a class="btn btn-sm btn-outline-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailShipped" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-cancel" role="tabpanel" aria-labelledby="listCancel">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListCancel" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailCancel" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-unshipment" role="tabpanel" aria-labelledby="listUnshipment">
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListUnshipment" CssClass="table table-striped table-hover align-middle order-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-Wrap="true">
                                                                <ItemTemplate>
                                                                    <%# BindCustomerText(Eval("CustomerName").ToString(), Eval("OperatorName").ToString()) %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                            <asp:BoundField DataField="ProductionDate" HeaderText="Production" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li>
                                                                            <a class="dropdown-item" id="aDetailUnshipment" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id").ToString()) %>'>Detail</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id").ToString()) %>'>Edit</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCopy(Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDuplicateOrder" onclick='<%# String.Format("return dataDuplicateOrder(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("CustomerId").ToString()) %>'>Copy / Duplicate</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Unsubmit Order", Eval("Status").ToString()) %>'>Unsubmit Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleNewOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "New Order", Eval("Status").ToString()) %>'>New Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Production Order", Eval("Status").ToString()) %>'>Production Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Hold Order", Eval("Status").ToString()) %>'>Hold Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return dataShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Receive Payment", Eval("Status").ToString()) %>'>Receive Payment</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Complete Order", Eval("Status").ToString()) %>'>Complete Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return dataCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleSurat(Eval("Status").ToString(), Eval("CompanyId").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Surat Jalan", Eval("Status").ToString()) %>'>Surat Jalan</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleDownloadBOE(Eval("Status").ToString(), Eval("Download").ToString(), Eval("Active")) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Download BOE", "") %>'>Download BOE</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleChina(Eval("Active"), Eval("Status").ToString(), Eval("OrderFactory").ToString()) %>'>
                                                                            <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalOcean" onclick='<%# String.Format("return dataOcean(`{0}`);", Eval("Id").ToString()) %>'>Shutter Ocean</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisibleLog() %>'>
                                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer order-filter-footer">
                                <div class="d-flex align-items-center" runat="server" id="divActive">
                                    <div class="ms-auto">
                                        <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlActive_SelectedIndexChanged">
                                            <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                            <asp:ListItem Value="0" Text="Non Active"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalShipment" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Detail Shipment</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover">
                            <tr>
                                <th>Shipment Number</th>
                                <th>Shipment Date</th>
                                <th>Container Number</th>
                                <th>Container ETA</th>
                                <th>Courier</th>
                            </tr>
                            <tr>
                                <td><span id="spanShipmentNumber"></span></td>
                                <td><span id="spanShipmentDate"></span></td>
                                <td><span id="spanContainerNumber"></span></td>
                                <td><span id="spanContainerEta"></span></td>
                                <td><span id="spanCourier"></span></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalBOEDownload" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">BOE Download</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover">
                            <tr>
                                <th>Status</th>
                                <th>Date</th>
                            </tr>
                            <tr>
                                <td><span id="spanBOEDownloadStatus"></span></td>
                                <td><span id="spanBOEDownloadDate"></span></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalStatusOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white" id="titleStatus"></h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtStatusOrderId" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtStatusOrderNew" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtStatusOrderOld" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnStatusOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnStatusOrder_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalDuplicateOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Dulicate Order</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtDuplicateOrderId" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtDuplicateOrderCustomerId" style="display:none;"></asp:TextBox>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Number (New)</label>
                            <asp:TextBox runat="server" ID="txtOrderNumberNew" CssClass="form-control" placeholder="Order Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Name (New)</label>
                            <asp:TextBox runat="server" ID="txtOrderNameNew" CssClass="form-control" placeholder="Order Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Note (New)</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtOrderNoteNew" Height="130px" CssClass="form-control" placeholder="Order Note ...." autocomplete="off" style="resize: none"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorDuplicateOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorDuplicateOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDuplicateOrder" CssClass="btn btn-primary" Text="Submit" OnClick="btnDuplicateOrder_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalShipmentOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Shipment Order</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtShipmentOrderId" style="display:none;"></asp:TextBox>
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
                    <div class="row mb-2" runat="server" id="divErrorShipmentOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorShipmentOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnShipmentOrder" CssClass="btn btn-primary" Text="Submit" OnClick="btnShipmentOrder_Click" />
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
                    <asp:TextBox runat="server" ID="txtCancelOrderId" style="display:none;"></asp:TextBox>
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
                    <asp:Button runat="server" ID="btnCancelOrder" CssClass="btn btn-danger" Text="Submit" OnClick="btnCancelOrder_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalOcean" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Shutter Ocean</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtOceanId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnOcean" CssClass="btn btn-info" Text="Confirm" OnClick="btnOcean_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
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
    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="selected_tab" runat="server" />

    <script type="text/javascript">
        $(document).ready(function () {
            var selectedTab = $("#<%=selected_tab.ClientID%>");
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "list-unsubmit";
            $('#divTabList a[href="#' + tabId + '"]').tab('show');

            $("#divTabList a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });
            $("#listUnsubmit").on("click", function () {
                updateSessionValue("list-unsubmit");
            });
            $(document).on("click", "#<%=listQuote.ClientID%>", function () {
                updateSessionValue("list-quote");
            });
            $("#listWaiting").on("click", function () {
                updateSessionValue("list-waiting");
            });
            $("#listSent").on("click", function () {
                updateSessionValue("list-sent");
            });
            $("#listReceive").on("click", function () {
                updateSessionValue("list-receive");
            });
            $("#listNew").on("click", function () {
                updateSessionValue("list-new");
            });
            $("#listProduction").on("click", function () {
                updateSessionValue("list-production");
            });
            $("#listHold").on("click", function () {
                updateSessionValue("list-hold");
            });
            $("#listShipped").on("click", function () {
                updateSessionValue("list-shipped");
            });
            $("#listCancel").on("click", function () {
                updateSessionValue("list-cancel");
            });
            $("#listUnshipment").on("click", function () {
                updateSessionValue("list-unshipment");
            });
        });
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            bindClickRow('<%= gvListUnsubmit.ClientID %>', 'aDetailUnsubmit');
            bindClickRow('<%= gvListQuote.ClientID %>', 'aDetailQuote');
            bindClickRow('<%= gvListWaiting.ClientID %>', 'aDetailWaiting');
            bindClickRow('<%= gvListSent.ClientID %>', 'aDetailSent');
            bindClickRow('<%= gvListReceive.ClientID %>', 'aDetailReceive');
            bindClickRow('<%= gvListNew.ClientID %>', 'aDetailNew');
            bindClickRow('<%= gvListProduction.ClientID %>', 'aDetailProduction');
            bindClickRow('<%= gvListHold.ClientID %>', 'aDetailHold');
            bindClickRow('<%= gvListShipped.ClientID %>', 'aDetailShipped');
            bindClickRow('<%= gvListCancel.ClientID %>', 'aDetailCancel');
            bindClickRow('<%= gvListUnshipment.ClientID %>', 'aDetailUnshipment');
        });
        function bindClickRow(gridId, detailId) {
            const gv = document.getElementById(gridId);
            if (!gv) return;

            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = "pointer";

                row.onclick = function (e) {
                    if (e.target.closest("a, button, [data-bs-toggle]")) return;

                    const btn = this.querySelector(`a[id*='${detailId}']`);
                    if (btn) btn.click();
                };
            }
        }
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
                bindClickRow('<%= gvListUnsubmit.ClientID %>', 'aDetailUnsubmit');
                bindClickRow('<%= gvListQuote.ClientID %>', 'aDetailQuote');
                bindClickRow('<%= gvListWaiting.ClientID %>', 'aDetailWaiting');
                bindClickRow('<%= gvListSent.ClientID %>', 'aDetailSent');
                bindClickRow('<%= gvListReceive.ClientID %>', 'aDetailReceive');
                bindClickRow('<%= gvListNew.ClientID %>', 'aDetailNew');
                bindClickRow('<%= gvListProduction.ClientID %>', 'aDetailProduction');
                bindClickRow('<%= gvListHold.ClientID %>', 'aDetailHold');
                bindClickRow('<%= gvListShipped.ClientID %>', 'aDetailShipped');
                bindClickRow('<%= gvListCancel.ClientID %>', 'aDetailCancel');
                bindClickRow('<%= gvListUnshipment.ClientID %>', 'aDetailUnshipment');
            });
        }
        function updateSessionValue(session) {
            $.ajax({
                type: "POST",
                url: "Default.aspx/UpdateSession",
                data: JSON.stringify({ value: session }),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
        }
        function dataShipment(number, date, container, coneta, courier) {
            document.getElementById("spanShipmentNumber").innerText = number;
            document.getElementById("spanShipmentDate").innerText = date;
            document.getElementById("spanContainerNumber").innerText = container;
            document.getElementById("spanContainerEta").innerText = coneta;
            document.getElementById("spanCourier").innerText = courier;
        }
        function dataBOEDownload(status, date) {
            document.getElementById("spanBOEDownloadStatus").innerText = status;
            document.getElementById("spanBOEDownloadDate").innerText = date;
        }
        function dataStatusOrder(id, status, oldStatus) {
            document.getElementById("titleStatus").textContent = status;
            document.getElementById("<%=txtStatusOrderId.ClientID %>").value = id;
            document.getElementById("<%=txtStatusOrderNew.ClientID %>").value = status;
            document.getElementById("<%=txtStatusOrderOld.ClientID %>").value = oldStatus;            
        }
        function dataDuplicateOrder(id, customerid) {
            document.getElementById("<%=txtDuplicateOrderId.ClientID %>").value = id;            
            document.getElementById("<%=txtDuplicateOrderCustomerId.ClientID %>").value = customerid;            
        }
        function showDuplicateOrder() {
            $("#modalDuplicateOrder").modal("show");
        }
        function dataShipmentOrder(id) {
            document.getElementById("<%=txtShipmentOrderId.ClientID %>").value = id;
        }
        function showShipmentOrder() {
            $("#modalShipmentOrder").modal("show");
        }
        function dataCancelOrder(id) {
            document.getElementById("<%=txtCancelOrderId.ClientID %>").value = id;
        }
        function showCancelOrder() {
            $("#modalCancelOrder").modal("show");
        }
        function dataOcean(id) {
            document.getElementById("<%=txtOceanId.ClientID %>").value = id;
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
        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 5000);
            return true;
        }
        ["modalBOEDownload", "modalShipment", "modalStatusOrder", "modalDuplicateOrder", "modalCancelOrder", "modalShipmentOrder", "modalOcean", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
