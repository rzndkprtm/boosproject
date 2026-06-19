<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Order_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="List Order" %>

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
                <asp:Button runat="server" ID="btnRework" CssClass="btn btn-danger" Text="Rework Order" OnClick="btnRework_Click" />
                <asp:Button runat="server" ID="btnFile" CssClass="btn btn-secondary" Text="File" OnClick="btnFile_Click" />
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-content">
                                <div class="card-header">
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                            <div class="input-group">
                                                <span class="input-group-text">Status</span>
                                                <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                            <div class="input-group" runat="server" id="divType">
                                                <span class="input-group-text">Order Type</span>
                                                <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                            <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Search</span>
                                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder="Order ID, Customer Name, Order Number, Order Name ....." autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="row mb-1" runat="server" id="divCompany">
                                        <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                            <div class="input-group">
                                                <span class="input-group-text">Company</span>
                                                <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Visible="false" OnPageIndexChanging="gvList_PageIndexChanging" OnDataBound="gvList_DataBound">
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
                                                <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-Wrap="true" />
                                                <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy}" />
                                                <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                <asp:BoundField DataField="ProductionDate" HeaderText="Production" DataFormatString="{0:dd MMM yyyy}" />
                                                <asp:BoundField DataField="OrderFactory" HeaderText="Factory" ItemStyle-Wrap="true" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Shipment">
                                                    <ItemTemplate>
                                                        <a class="btn btn-sm btn-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipment" onclick='<%# String.Format("return dataShipment(`{0}`, `{1:dd MMM yyyy}`, `{2}`, `{3:dd MMM yyyy}`, `{4}`);", Eval("ShipmentNumber").ToString(), Eval("ShipmentDate"), Eval("ContainerNumber").ToString(), Eval("ContainerETA"), Eval("Courier").ToString()) %>'>Show</a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="BOE">
                                                    <ItemTemplate>
                                                        <a class="btn btn-sm btn-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalBOEDownload" onclick='<%# String.Format("return dataBOEDownload(`{0}`, `{1:dd MMM yyyy HH:mm:ss}`);", Eval("Download").ToString(), Eval("DownloadDate")) %>'>Show</a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li>
                                                                <a class="dropdown-item" id="aDetail" href='<%# Page.ResolveUrl("~/order/detail?orderid=" & Eval("Id")) %>'>Detail</a>
                                                            </li>
                                                            <li runat="server" visible='<%# VisibleEdit(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                <a class="dropdown-item" href='<%# Page.ResolveUrl("~/order/edit?orderid=" & Eval("Id")) %>'>Edit</a>
                                                            </li>
                                                            <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                <a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return dataStatusOrder(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), "Delete Order", "") %>'>Delete</a>
                                                            </li>
                                                            <li runat="server" visible='<%# VisibleCopy(Eval("Active").ToString()) %>'>
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
                                    <div class="d-flex justify-content-end mt-3">
                                        <nav id="navPager" runat="server" visible="false">
                                            <ul class="pagination pagination mb-0">
                                                <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                                    <ItemTemplate>
                                                        <li class='page-item <%# Eval("CssClass") %>'>
                                                            <asp:LinkButton runat="server" ID="lnkPage" CssClass="page-link" Text='<%# Eval("Text") %>' CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>' />
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ul>
                                        </nav>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer">
                                <div class="d-flex" runat="server" id="divActive">
                                    <div class="ms-auto">
                                        <div class="ms-2 d-inline-block">
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
                    <asp:TextBox runat="server" ID="txtStatusOrderId"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtStatusOrderNew"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtStatusOrderOld"></asp:TextBox>
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

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
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
                bindGridRowClick();
            });
        }
        function bindGridRowClick() {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) return;
            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = "pointer";
                row.onclick = function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                        return;
                    }
                    const btn = this.querySelector("a[id*='aDetail']");
                    if (btn) btn.click();
                };
            }
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            bindGridRowClick();
        });
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
