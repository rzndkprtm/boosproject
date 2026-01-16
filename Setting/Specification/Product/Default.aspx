<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Specification_Product_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Product" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification">Specification</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row mb-3">
            <div class="col-12 d-flex justify-content-end">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary me-1" Text="Add New" OnClick="btnAdd_Click" />
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                    <asp:DropDownList runat="server" ID="ddlDesignSort" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesignSort_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-4 mb-2">
                                    <asp:DropDownList runat="server" ID="ddlBlindSort" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlBlindSort_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-5 d-flex justify-content-end">
                                    <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                        <div class="input-group">
                                            <span class="input-group-text">Search : </span>
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                            <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="row mb-2" runat="server" id="divError">
                                <div class="col-12">
                                    <div class="alert alert-danger">
                                        <span runat="server" id="msgError"></span>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="DesignName" HeaderText="Design Name" />
                                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                                <asp:TemplateField HeaderText="Company Detail">
                                                    <ItemTemplate>
                                                        <%# BindCompanyDetail(Eval("Id").ToString()) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                        <ul class="dropdown-menu">
                                                            <li runat="server" visible='<%# PageAction("Detail") %>'>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Copy") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalCopy" onclick='<%# String.Format("return showCopy(`{0}`);", Eval("Id").ToString()) %>'>Copy</a>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Active") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalActive" onclick='<%# String.Format("return showActive(`{0}`, `{1}`);", Eval("Id").ToString(), Convert.ToInt32(Eval("Active"))) %>'><%# TextActive(Convert.ToInt32(Eval("Active"))) %></a>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Delete") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
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
                    </div>

                    <div class="card-footer">
                        <div class="d-flex">
                            <div class="ms-2 d-inline-block">
                                <asp:DropDownList runat="server" ID="ddlCompanyDetailSort" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompanyDetailSort_SelectedIndexChanged"></asp:DropDownList>
                            </div>
                            <div class="ms-2 d-inline-block">
                                <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlActive_SelectedIndexChanged">
                                    <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="Non Active"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalCopy" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-secondary">
                    <h5 class="modal-title white">Copy Product</h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdCopy" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCopy" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnCopy_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Product</h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDelete_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalActive" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white" id="titleModalLabelActive"></h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdActive" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtActive" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnActive" CssClass="btn btn-info" Text="Confirm" OnClick="btnActive_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
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

        function showCopy(id) {
            document.getElementById('<%=txtIdCopy.ClientID %>').value = id;
        }
        function showDelete(id) {
            document.getElementById('<%=txtIdDelete.ClientID %>').value = id;
        }
        function showLog() {
            $("#modalLog").modal("show");
        }
        function showActive(id, active) {
            let title = "Activate Product";
            if (active === 1) { title = "Deactivate Product"; }

            document.getElementById("titleModalLabelActive").innerHTML = title;
            document.getElementById('<%=txtIdActive.ClientID %>').value = id;
            document.getElementById('<%=txtActive.ClientID %>').value = active;
        }

        ["modalCopy", "modalActive", "modalDelete", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblIdNew"></asp:Label>
        <asp:Label runat="server" ID="lblActive"></asp:Label>
    </div>
</asp:Content>
