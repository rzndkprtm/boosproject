<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Order_Rework_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Rework Detail" %>
<%@ Import Namespace="System.Web" %>

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
                <a href="#" runat="server" id="aCancelRework" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalCancelRework">Cancel</a>
                <a href="#" runat="server" id="aSubmitRework" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalSubmitRework">Submit</a>
                <a href="#" runat="server" id="aApproveRework" class="btn btn-success me-1" data-bs-toggle="modal" data-bs-target="#modalApproveRework">Approve</a>
                <a href="#" runat="server" id="aRejectRework" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalRejectRework">Reject</a>
            </div>
        </section>

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
        
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-2">
                                <div class="col-12 col-sm-12 col-lg-9 mb-2">
                                    <label>Customer Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCustomerName" CssClass="font-bold"></asp:Label>
                                    <asp:Label runat="server" ID="lblCustomerId" Visible="false"></asp:Label>
                                    <asp:Label runat="server" ID="lblCompanyId" Visible="false"></asp:Label>
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
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-12 col-sm-12 col-lg-4">
                <div class="row">
                    <div class="col-12">
                        <div class="card">
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-6">
                                            <label>Created Date</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblCreatedDate" CssClass="font-bold"></asp:Label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6">
                                            <label>Created By</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblCreatedBy" CssClass="font-bold"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-6">
                                            <label>Rework Status</label>
                                            <br />
                                            <asp:Label runat="server" ID="lblStatus" CssClass="font-bold"></asp:Label>
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
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-7">
                                    <h3 class="card-title">Your Item</h3>
                                </div>
                                <div class="col-5 d-flex justify-content-end">
                                    <asp:Button runat="server" ID="btnAddItem" CssClass="btn btn-primary" Text="Add Item" OnClick="btnAddItem_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="accordion" id="accordionExample">
                                <asp:Repeater runat="server" ID="rptRework" OnItemDataBound="rptRework_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="accordion-item">
                                            <h2 class="accordion-header" id="heading<%# Container.ItemIndex %>">
                                                <button class="accordion-button <%# IIf(Container.ItemIndex > 0, "collapsed", "") %>" type="button" data-bs-toggle="collapse" data-bs-target="#collapse<%# Container.ItemIndex %>" aria-expanded="<%# IIf(Container.ItemIndex = 0, "true", "false") %>" aria-controls="collapse<%# Container.ItemIndex %>"><%# Eval("TitleItem") %>
                                                </button>
                                            </h2>
                                            <div id="collapse<%# Container.ItemIndex %>" class="accordion-collapse collapse <%# IIf(Container.ItemIndex = 0, "show", "") %>" aria-labelledby="heading<%# Container.ItemIndex %>" data-bs-parent="#accordionExample">
                                                <div class="accordion-body">
                                                    <div class="row">
                                                        <div class="col-8">
                                                            <div class="row mb-3">
                                                                <div class="col-12 col-sm-12 col-lg-2">
                                                                    <label>Category</label>
                                                                </div>
                                                                <div class="col-12 col-sm-12 col-lg-8">
                                                                    <%# Eval("Category") %>
                                                                    <span runat="server" visible='<%# VisibleDetailRework() %>'>
                                                                        <a class="btn btn-sm" href="javascript:void(0)" data-bs-toggle="modal" data-bs-target="#modalCategory" onclick="showCategory('<%# Eval("Id") %>', '<%# Eval("Category") %>')"><i class="bi bi-pencil-square"></i></a>
                                                                    </span>
                                                                </div>
                                                            </div>

                                                            <div class="row mb-3">
                                                                <div class="col-12 col-sm-12 col-lg-2">
                                                                    <label>Description</label>
                                                                </div>
                                                                <div class="col-12 col-sm-12 col-lg-6">
                                                                    <asp:Literal runat="server" Text='<%# Eval("Description").ToString().Replace(vbCrLf, "<br/>").Replace(vbLf, "<br/>") %>' Mode="PassThrough"></asp:Literal>
                                                                    <span runat="server" visible='<%# VisibleDetailRework() %>'>
                                                                        <a class="btn btn-sm" href="javascript:void(0)" data-bs-toggle="modal" data-bs-target="#modalDescription" onclick='showDescription("<%# Eval("Id") %>", "<%# HttpUtility.JavaScriptStringEncode(Eval("Description").ToString()) %>")'>
                                                                            <i class="bi bi-pencil-square"></i>
                                                                        </a>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-4 text-end" runat="server" visible='<%# VisibleDetailRework() %>'>
                                                            <a href="javascript:void(0)" class="btn btn-sm btn-danger" data-bs-toggle="modal" data-bs-target="#modalDeleteItem" onclick="showDeleteItem('<%# Eval("Id") %>')">Delete Item</a>
                                                        </div>
                                                    </div>
                                                    
                                                    <div class="row mb-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <asp:GridView runat="server" ID="gvFiles" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" CssClass="table table-bordered table-hover mb-0" OnRowCommand="gvFiles_RowCommand">
                                                                    <RowStyle />
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

                                                    <div runat="server" class="row mb-3" visible='<%# VisibleDetailRework() %>'>
                                                        <div class="col-3">
                                                            <a href="javascript:void(0)" class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalUpload" onclick="showUpload('<%# Eval("Id") %>')">Upload New File</a>
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
        </section>
    </div>

    <div class="modal fade" id="modalCategory" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Update Category</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtCategoryId" style="display:none;"></asp:TextBox>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Category</label>
                            <asp:DropDownList runat="server" ID="ddlCategory" CssClass="form-select">
                                <asp:ListItem Value="Product Fault" Text="Product Fault"></asp:ListItem>
                                <asp:ListItem Value="Warranty Issue" Text="Warranty Issue"></asp:ListItem>
                                <asp:ListItem Value="Freight Damage to Customer" Text="Freight Damage to Customer"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCategory" CssClass="btn btn-danger" Text="Submit" OnClick="btnCategory_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalDescription" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Update Description</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtDescriptionId" style="display:none;"></asp:TextBox>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDescription" CssClass="btn btn-danger" Text="Submit" OnClick="btnDescription_Click" OnClientClick="return showWaiting();" />
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUpload" CssClass="btn btn-danger" Text="Submit" OnClick="btnUpload_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalCancelRework" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Cancel Rework</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCancelRework" CssClass="btn btn-danger" Text="Confirm" OnClick="btnCancelRework_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitRework" CssClass="btn btn-success" Text="Confirm" OnClick="btnSubmitRework_Click" OnClientClick="return showWaiting();" />
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnRejectRework" CssClass="btn btn-danger" Text="Confirm" OnClick="btnRejectRework_Click" OnClientClick="return showWaiting();" />
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteItem" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteItem_Click" OnClientClick="return showWaiting();" />
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
    </div>

    <script type="text/javascript">
        [
            "modalWaiting",
            "modalCategory", "modalDescription", "modalUpload",
            "modalCancelRework", "modalSubmitRework",
            "modalApproveRework", "modalRejectRework",
            "modalDeleteItem"
        ].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        function showCategory(id, category) {
            document.getElementById("<%=txtCategoryId.ClientID %>").value = id;
            document.getElementById("<%=ddlCategory.ClientID %>").value = category;
        }

        function showDescription(id, description) {
            document.getElementById("<%=txtDescriptionId.ClientID %>").value = id;
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
            }, 6000);
        }

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
