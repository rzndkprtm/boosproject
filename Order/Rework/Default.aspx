<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Order_Rework_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="List Rework" %>

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

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-lg-3 col-md-12 col-sm-12 mb-2">
                                    <div class="input-group">
                                        <asp:Label runat="server" CssClass="input-group-text" Text="Status"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                            <asp:ListItem Value="" Text="All Reworks"></asp:ListItem>
                                            <asp:ListItem Value="Unsubmitted" Text="Unsubmitted"></asp:ListItem>
                                            <asp:ListItem Value="Pending Approval" Text="Pending Approval"></asp:ListItem>
                                            <asp:ListItem Value="Approved" Text="Approved"></asp:ListItem>
                                            <asp:ListItem Value="Rejected" Text="Rejected"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-9 col-md-12 col-sm-12 d-flex justify-content-end">
                                    <asp:Panel runat="server" DefaultButton="btnSearch" Width="50%">
                                        <div class="input-group">
                                            <span class="input-group-text">Search</span>
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder=""></asp:TextBox>
                                            <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:TemplateField HeaderText="New Order ID">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" CommandName="ToOrder" CommandArgument='<%# Eval("HeaderIdNew") %>'><%# GetOrderId(Eval("HeaderIdNew").ToString()) %></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" ItemStyle-Wrap="true" />
                                                <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-Wrap="true" />
                                                <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy}" />
                                                <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton runat="server" ID="linkLog" CssClass="dropdown-item" Text="Log" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
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

                        <div class="card-footer">
                            <div class="row" runat="server" id="divActive">
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select" AutoPostBack="true">
                                        <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Non Active"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Changelog</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
    
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorLog">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorLog"></span>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListLogs" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA LOG NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeader="false" GridLines="None" BorderStyle="None">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# BindTextLog(Eval("Id").ToString()) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvList.ClientID %>');
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

        ["modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        function showLog() {
            $("#modalLog").modal("show");
        }

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>