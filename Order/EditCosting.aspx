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
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <h4 class="card-title" runat="server" id="hTitle"></h4>
                        </div>
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" DataKeyNames="Id">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                                <asp:TemplateField HeaderText="New Price (Buy)" ItemStyle-Width="180px">
                                                    <ItemTemplate>
                                                        <div class="input-group">
                                                            <span runat="server" id="spanEditBuyPrice" class="input-group-text">$</span>
                                                            <asp:TextBox runat="server" ID="txtNewBuyPrice" CssClass="form-control" Text='<%# String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##}", Eval("BuyPrice")) %>'></asp:TextBox>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="New Price (Sell)" ItemStyle-Width="180px">
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
                            </div>
                            <div class="row" runat="server" id="divNote">
                                <div class="col-12">
                                    <div class="divider divider-left-center">
                                        <div class="divider-text">Additional Note</div>
                                    </div>
                                </div>
                                <div class="col-12">
                                    <asp:Label runat="server" ID="lblNote">Test</asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-5">
                                    <a runat="server" id="aAdd" class="btn btn-secondary" href="#" data-bs-toggle="modal" data-bs-target="#modalNote">Add Note</a>
                                    <a runat="server" id="aDelete" class="btn btn-danger me-1" href="#" data-bs-toggle="modal" data-bs-target="#modalDeleteNote">Delete Note</a>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-7 d-flex justify-content-end gap-1">
                                    <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click"  />
                                    <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel / Close" OnClick="btnCancel_Click" />
                                </div>
                             </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-4">
                <div class="card">
                    <div class="card-content"></div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal modal-blur fade" id="modalNote" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Note</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">Additional Note</label>
                            <asp:TextBox runat="server" ID="txtNote" CssClass="form-control" placeholder="Note ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnNote" CssClass="btn btn-primary" Text="Submit" OnClick="btnNote_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteNote" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Note</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteNote" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteNote_Click" />
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
        ["modalNote", "modalDeleteNote"].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
