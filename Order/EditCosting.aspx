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
                                    <asp:Label runat="server" ID="lblNote"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="card-footer">
                            <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel / Close" OnClick="btnCancel_Click" />
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblOrderStatus"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyId"></asp:Label>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
