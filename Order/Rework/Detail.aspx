<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Order_Rework_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Rework Detail" %>
<%@ Import Namespace="System.Web" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .accordion-button::after{ display:none!important; }
        .card{ border:none; border-radius:12px; box-shadow:0 3px 12px rgba(0,0,0,.08); }
        .card-header{ background:#fff; border-bottom:1px solid #ececec; }
        .info-row{ display:flex; justify-content:space-between; align-items:center; padding:10px 0; border-bottom:1px solid #f1f1f1; }
        .info-row:last-child{ border-bottom:none; }
        .info-label{ color:#6c757d; font-weight:600; }
        .info-value{ font-weight:700; text-align:right; }
        .description-box{ background:#fafafa; border-left:4px solid #435ebe; border-radius:8px; padding:12px; white-space:pre-line; }
        .action-bar{ position:sticky; top:0; z-index:100; background:#fff; padding:15px; border-radius:10px; box-shadow:0 2px 10px rgba(0,0,0,.08); margin-bottom:20px; }
        .accordion-item{ border:none; border-radius:10px; overflow:hidden; margin-bottom:18px; box-shadow:0 2px 8px rgba(0,0,0,.08); }
        .accordion-button{ background:#f8f9fa; font-weight:700; padding:18px; }
        .table td{ vertical-align:middle; }
        .badge-status{ font-size:.9rem; padding:.5rem .8rem; }
        .table thead{  background:#435ebe; color:white; }
        .table-hover tbody tr:hover{ background:#f7f9ff; }
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
                            <li class="breadcrumb-item"><a runat="server" href="~/order/rework">Rework</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row mb-4">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <a href="javascript:void(0);" runat="server" id="aSubmitRework" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalSubmitRework">Submit</a>
                <a href="javascript:void(0);" runat="server" id="aDeleteRework" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalDeleteRework">Delete</a>
                <a href="javascript:void(0);" runat="server" id="aCancelRework" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalCancelRework">Cancel</a>
                <a href="javascript:void(0);" runat="server" id="aApproveRework" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalApproveRework">Approve</a>
                <a href="javascript:void(0);" runat="server" id="aRejectRework" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalRejectRework">Reject</a>
                <a runat="server" href="~/order/rework" class="btn btn-secondary me-1">Close</a>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-12">
                <div class="row mb-2" runat="server" id="divError">
                    <div class="col-12">
                        <div class="alert alert-danger shadow-sm">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-4">
                <div class="row">
                    <div class="col-12">
                        <div class="card">
                            <div class="card-content">
                                <div class="card-body">
                                    <h5 class="mb-4">Order Information</h5>
                                    <div class="info-row">
                                        <span class="info-label">👤 Customer</span>
                                        <span class="info-value">
                                            <asp:Label runat="server" ID="lblCustomerName"></asp:Label>
                                        </span>
                                    </div>
                                    <div class="info-row">
                                        <span class="info-label">📦 Order #</span>
                                        <span class="info-value">
                                            <asp:Label runat="server" ID="lblOrderId"></asp:Label>
                                        </span>
                                    </div>
                                    <div class="info-row">
                                        <span class="info-label">🔖 Order Number</span>
                                        <span class="info-value">
                                            <asp:Label runat="server" ID="lblOrderNumber"></asp:Label>
                                        </span>
                                    </div>
                                    <div class="info-row">
                                        <span class="info-label">📝 Order Name</span>
                                        <span class="info-value">
                                            <asp:Label runat="server" ID="lblOrderName"></asp:Label>
                                        </span>
                                    </div>
                                    <div class="info-row">
                                        <span class="info-label">📅 Created</span>
                                        <span class="info-value">
                                            <asp:Label runat="server" ID="lblCreatedDate"></asp:Label>
                                        </span>
                                    </div>
                                    <div class="info-row">
                                        <span class="info-label">👨 Created By</span>
                                        <span class="info-value">
                                            <asp:Label runat="server" ID="lblCreatedBy"></asp:Label>
                                        </span>
                                    </div>
                                    <div class="info-row">
                                        <span class="info-label">🚦 Status</span>
                                        <span class="info-value">
                                            <span class="badge bg-success badge-status">
                                                <asp:Label runat="server" ID="lblStatus"></asp:Label>
                                            </span>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="row">
                    <div class="col-12">
                        <div class="card">
                            <div class="card-content">
                                <div class="card-header">
                                    <div class="d-flex justify-content-between align-items-center flex-wrap">
                                        <div>
                                            <h4 class="mb-0">Your Rework Items</h4>
                                            <small class="text-muted">Manage all rework requests below</small>
                                        </div>
                                        <div>
                                            <a href="javascript:void(0);" runat="server" id="aAddItem" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalAddItem">Add Item</a>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="accordion" id="accordionExample">
                                        <asp:Repeater runat="server" ID="rptRework" OnItemDataBound="rptRework_ItemDataBound">
                                            <ItemTemplate>
                                                <div class="accordion-item">
                                                    <div class="card mb-3 shadow-sm border-0">
                                                        <div class="card-header bg-light">
                                                            <div class="d-flex justify-content-between align-items-center">
                                                                <div>
                                                                    <h5 class="mb-1"><%# Eval("TitleItem") %></h5>
                                                                    <small class="text-muted"><%# Eval("Category") %></small>
                                                                </div>
                                                                <span class="badge bg-primary">
                                                                    <%# Eval("InstallDate","{0:dd MMM yyyy}") %>
                                                                </span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="accordion-collapse show">
                                                        <div class="accordion-body">
                                                            <div class="row">
                                                                <div class="col-8">
                                                                    <div class="row mb-3">
                                                                        <div class="col-12 col-sm-12 col-lg-3">
                                                                            <label>Description :</label>
                                                                        </div>
                                                                        <div class="col-12 col-sm-12 col-lg-6">
                                                                            <asp:Literal runat="server" Text='<%# Eval("Description").ToString().Replace(vbCrLf,"<br/>").Replace(vbLf,"<br/>") %>' Mode="PassThrough"></asp:Literal>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="col-4 text-end" runat="server" visible='<%# VisibleDetailRework() %>'>
                                                                    <a href="javascript:void(0);" class="btn btn-sm btn-secondary" data-bs-toggle="modal" data-bs-target="#modalUpdateItem" onclick="showUpdateItem('<%# Eval("Id") %>','<%# Eval("Category") %>', '<%# Eval("InstallDate","{0:yyyy-MM-dd}") %>', '<%# HttpUtility.JavaScriptStringEncode(Eval("Description").ToString()) %>')">Update Item</a>
                                                                    <a href="javascript:void(0);" class="btn btn-sm btn-danger" data-bs-toggle="modal" data-bs-target="#modalDeleteItem" onclick="showDeleteItem('<%# Eval("Id") %>')"> Delete Item</a>
                                                                </div>
                                                            </div>
                                                            <div class="row mt-2 mb-3">
                                                                <div class="col-12">
                                                                    <div class="table-responsive">
                                                                        <asp:GridView runat="server" ID="gvFiles" AutoGenerateColumns="false" ShowHeaderWhenEmpty="True" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" CssClass="table table-bordered table-hover mb-0" OnRowCommand="gvFiles_RowCommand">
                                                                            <Columns>
                                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="70px">
                                                                                    <ItemTemplate>
                                                                                        <%# Container.DataItemIndex + 1 %>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:BoundField DataField="FileName" HeaderText="File Name" />
                                                                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="150px">
                                                                                    <ItemTemplate>
                                                                                        <a runat="server" class="btn btn-sm btn-primary" href='<%# Eval("FilePath") %>' target="_blank">View</a>
                                                                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-danger" Text="Delete" CommandName="DeleteFile" CommandArgument='<%# Eval("FilePath") %>' Visible='<%# VisibleDetailRework() %>'></asp:LinkButton>
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row mb-3">
                                                                <div class="col-8">
                                                                    <div runat="server" visible='<%# VisibleDetailRework() %>'>
                                                                        <a href="javascript:void(0);" class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalUpload" onclick="showUpload('<%# Eval("Id") %>')">Upload New File</a>
                                                                    </div>
                                                                    <div runat="server" visible='<%# VisibleDownloadZip() %>'>
                                                                        <asp:LinkButton runat="server" CssClass="btn btn-sm btn-success ms-2" Text="Download All" CommandName="DownloadZip" CommandArgument='<%# Eval("Id") %>' OnCommand="DownloadZip_Command"></asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
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

    <div class="modal fade text-center" id="modalSubmitRework" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Submit Rework</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitRework" CssClass="btn btn-success" Text="Confirm" OnClick="btnSubmitRework_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteRework" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Rework</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteRework" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteRework_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalApproveRework" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Approve Rework</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnApproveRework" CssClass="btn btn-success" Text="Confirm" OnClick="btnApproveRework_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalRejectRework" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Reject Rework</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnRejectRework" CssClass="btn btn-danger" Text="Confirm" OnClick="btnRejectRework_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalAddItem" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-full modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Rework Order</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListAddItem" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" DataKeyNames="Id">
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
                                <asp:TemplateField HeaderText="Category">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlAddCategory" runat="server" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Customer Error" Text="Customer Error"></asp:ListItem>
                                            <asp:ListItem Value="Product Fault" Text="Product Fault"></asp:ListItem>
                                            <asp:ListItem Value="Warranty Issue" Text="Warranty Issue"></asp:ListItem>
                                            <asp:ListItem Value="Freight Damage to Customer" Text="Freight Damage to Customer"></asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Install Date">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtAddInstallDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtAddDescription" runat="server" CssClass="form-control"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div class="mt-2 alert alert-danger" runat="server" id="divErrorAddItem">
                        <span runat="server" id="msgErrorAddItem"></span>
                    </div>
                </div>
                <div class="modal-footer ">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                    <asp:Button runat="server" ID="btnAddItem" CssClass="btn btn-danger" Text="Submit" OnClick="btnAddItem_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalUpdateItem" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Update Category</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtDetailId" style="display:none;"></asp:TextBox>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Category</label>
                            <asp:DropDownList runat="server" ID="ddlCategory" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Customer Error" Text="Customer Error"></asp:ListItem>
                                <asp:ListItem Value="Product Fault" Text="Product Fault"></asp:ListItem>
                                <asp:ListItem Value="Warranty Issue" Text="Warranty Issue"></asp:ListItem>
                                <asp:ListItem Value="Freight Damage to Customer" Text="Freight Damage to Customer"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Install Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtInstallDate" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="UpdateItem" CssClass="btn btn-success" Text="Submit" OnClick="UpdateItem_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteItem" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Item</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdDeleteItem" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteItem" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteItem_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalUpload" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Update File</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtUploadId" style="display:none;"></asp:TextBox>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Choose File</label>
                            <asp:FileUpload runat="server" ID="fuFile" CssClass="form-control" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUpload" CssClass="btn btn-danger" Text="Submit" OnClick="btnUpload_Click" OnClientClick="return showWaiting();" />
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
        <asp:Label runat="server" ID="lblReworkId"></asp:Label>
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyId"></asp:Label>
    </div>

    <script type="text/javascript">
        ["modalSubmitRework", "modalDeleteRework", "modalApproveRework", "modalRejectRework", "modalWaiting", "modalAddItem", "modalUpdateItem", "modalDeleteItem", "modalUpload"].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        function toggleSelectAll(source) {
            var gv = document.getElementById("<%= gvListAddItem.ClientID %>");
            var checkBoxes = gv.querySelectorAll("input[type='checkbox'][id*='chkSelect']");
            checkBoxes.forEach(function (cb) {
                cb.checked = source.checked;
            });
        }
        function showAddItem() {
            $("#modalAddItem").modal("show");
        }
        function showUpdateItem(id, category, install, description) {
            document.getElementById("<%=txtDetailId.ClientID %>").value = id;
            document.getElementById("<%=ddlCategory.ClientID %>").value = category;
            document.getElementById("<%=txtInstallDate.ClientID %>").value = install;
            document.getElementById("<%=txtDescription.ClientID %>").value = description;
        }
        function showUpload(id) {
            document.getElementById("<%=txtUploadId.ClientID %>").value = id;
        }
        function showDeleteItem(id) {
            document.getElementById("<%=txtIdDeleteItem.ClientID %>").value = id;
        }
        function showWaiting() {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
            }, 2000);
        }
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>