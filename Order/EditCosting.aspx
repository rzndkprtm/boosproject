<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditCosting.aspx.vb" Inherits="Order_EditCosting" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Costing" %>

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
                            <li class="breadcrumb-item"><a href='<%= ResolveUrl("~/order/detail?orderid=" & lblHeaderId.Text) %>'>Detail</a></li>
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
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title" runat="server" id="hTitle"></h4>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" DataKeyNames="Id">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" />
                                    <asp:BoundField DataField="Type" HeaderText="Type" />
                                    <asp:TemplateField HeaderText="Description">
                                        <ItemTemplate>
                                            <%# BindDescription(Eval("DesignName").ToString(), Eval("Description").ToString(), Eval("OrderNote").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Buy Price" ItemStyle-Width="180px">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txtNewBuyPrice" CssClass="form-control" Visible='<%# Eval("Type").ToString() <> "Note" %>' Text='<%# String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##}", Eval("BuyPrice")) %>'></asp:TextBox>
                                            <asp:Label runat="server" Text="-" Visible='<%# Eval("Type").ToString() = "Note" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sell Price" ItemStyle-Width="180px">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txtNewSellPrice" CssClass="form-control" Visible='<%# Eval("Type").ToString() <> "Note" %>' Text='<%# String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##}", Eval("SellPrice")) %>'></asp:TextBox>
                                            <asp:Label runat="server" Text="-" Visible='<%# Eval("Type").ToString() = "Note" %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="150px">
                                        <ItemTemplate>
                                            <a class="btn btn-success btn-sm" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalUpdate" onclick='<%# String.Format("return showUpdate(`{0}`);", Eval("Id").ToString()) %>'>Update</a>
                                            <a class="btn btn-danger btn-sm" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="card-footer d-flex justify-content-between align-items-center">
                        <div>
                            <button class="btn btn-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Add Costings</button>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalAddDiscount">Add Discount</a></li>
                                <li><a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalAddSurcharge">Add Surcharge</a></li>
                                <li><a class="dropdown-item" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalAddNote">Add Note</a></li>
                            </ul>
                            <a class="btn btn-warning" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalReset">Reset Costing</a>
                        </div>
                        <div>
                            <asp:Button runat="server" ID="btnFinish" CssClass="btn btn-primary" Text="Finish" OnClick="btnFinish_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalAddDiscount" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add Discount</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Discount</label>
                            <div class="input-group">
                                <asp:TextBox runat="server" ID="txtDiscount" CssClass="form-control" TextMode="Number" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                                <span class="input-group-text">%</span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Additional Note</label>
                            <asp:TextBox runat="server" ID="txtDiscountNote" CssClass="form-control" placeholder="Note ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddDiscount" CssClass="btn btn-secondary" Text="Submit" OnClick="btnAddDiscount_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalAddSurcharge" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add Surcharge</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Item</label>
                            <asp:DropDownList runat="server" ID="ddlAddItem" CssClass="form-select">
                                <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                <asp:ListItem Value="6" Text="6"></asp:ListItem>
                                <asp:ListItem Value="7" Text="7"></asp:ListItem>
                                <asp:ListItem Value="8" Text="8"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" ID="txtAddDescription" CssClass="form-control" placeholder="Description ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Buy Price</label>
                            <asp:TextBox runat="server" ID="txtAddBuyPrice" CssClass="form-control" placeholder="Buy Price ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Sell Price</label>
                            <asp:TextBox runat="server" ID="txtAddSellPrice" CssClass="form-control" placeholder="Sell Price ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddSurcharge" CssClass="btn btn-secondary" Text="Submit" OnClick="btnAddSurcharge_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalAddNote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Add Note</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" ID="txtNote" CssClass="form-control" placeholder="Description ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnAddNote" CssClass="btn btn-secondary" Text="Submit" OnClick="btnAddNote_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalReset" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white">Reset Price</h5>
                </div>
                <div class="modal-body text-center py-4">                    
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnReset" CssClass="btn btn-warning" Text="Confirm" OnClick="btnReset_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalUpdate" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Update Item</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtUpdateId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUpdate" CssClass="btn btn-success" Text="Confirm" OnClick="btnUpdate_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Item</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtDeleteId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDelete_Click" />
                </div>
            </div>
        </div>
    </div>
    
    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblOrderStatus"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyId"></asp:Label>
    </div>

    <script type="text/javascript">
        ["modalAddDiscount", "modalAddSurcharge", "modalAddNote", "modalReset", "modalUpdate", "modalDelete"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        function showUpdate(id) {
            document.getElementById("<%=txtUpdateId.ClientID %>").value = id;
        }
        function showDelete(id) {
            document.getElementById("<%=txtDeleteId.ClientID %>").value = id;
        }
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
