<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Costing.aspx.vb" Inherits="Sales_Costing" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Sales Costing" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Timer runat="server" ID="tmrTicket" Interval="5000" OnTick="tmrTicket_Tick" />
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">The Summary Date represents the Production Date of the order.</p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/sales">Sales</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row mb-2" runat="server" id="divError">
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
                        <div class="row">
                            <div class="col-12 col-sm-12 col-lg-8 mb-2">
                                <h5 class="card-title">Summary Sales</h5>
                            </div>
                            <div class="col-12 col-sm-12 col-lg-4 mb-2 d-flex justify-content-end">
                                <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                    <div class="input-group">
                                        <span class="input-group-text">Search</span>
                                        <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <asp:UpdatePanel runat="server" ID="upDetail" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="31" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="SummaryDate" HeaderText="Order Date" DataFormatString="{0:dd MMM yyyy}" />
                                                        <asp:TemplateField HeaderText="Total Cost Price">
                                                            <ItemTemplate>
                                                                <%# BindPrice(Eval("TotalCostPrice")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Total Selling Price">
                                                            <ItemTemplate>
                                                                <%# BindPrice(Eval("TotalSellingPrice")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Total Profit">
                                                            <ItemTemplate>
                                                                <%# BindPrice(Eval("TotalProfit")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Total Paid Amount">
                                                            <ItemTemplate>
                                                                <%# BindPrice(Eval("TotalPaidAmount")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Total Unpaid Amount">
                                                            <ItemTemplate>
                                                                <%# BindPrice(Eval("TotalUnpaidAmount")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <asp:LinkButton runat="server" ID="linkRefresh" CssClass="btn btn-primary btn-sm" Text="Refresh" CommandName="Refresh" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
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
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="tmrTicket" EventName="Tick" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                        </div>
                    </div>
                    <div class="card-footer text-center"></div>
                </div>
            </div>
        </section>
    </div>

    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            document.title = '<%= Page.Title %> - <%= Session("CompanyName") %>';
        });
    </script>
</asp:Content>