<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Specification_Bottom_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Bottom Type" %>

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
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                    <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
                                </div>
                                <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
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
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="BOTTOM RAIL DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                                <asp:TemplateField HeaderText="Design">
                                                    <ItemTemplate>
                                                        <%# BindDesign(Eval("Id").ToString()) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Company Detail">
                                                    <ItemTemplate>
                                                        <%# BindCompany(Eval("Id").ToString()) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Status" HeaderText="Status" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li runat="server" visible='<%# LoginAccess("Detail") %>'>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# LoginAccess("Edit") %>'>
                                                                <asp:LinkButton runat="server" ID="linkEdit" CssClass="dropdown-item" Text="Edit" CommandName="Ubah" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# LoginAccess("Change Status") %>'>
                                                                <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalChangeStatus" onclick='<%# String.Format("return showChangeStatus(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), Eval("Name").ToString(), Eval("Status").ToString()) %>'>Change Status</a>
                                                            </li>
                                                            <li>
                                                                <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('Bottoms', '<%# Eval("Id") %>')">Log</a>
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
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalChangeStatus" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Change Status</h4>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdStatus" style="display:none;"></asp:TextBox>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Old Status</label>
                            <asp:TextBox runat="server" ID="txtOldStatus" style="display:none;"></asp:TextBox>
                            <asp:DropDownList runat="server" ID="ddlOldStatus" ClientIDMode="Static" CssClass="form-select" Enabled="false">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">New Status</label>
                            <asp:DropDownList runat="server" ID="ddlNewStatus" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnChangeStatus" CssClass="btn btn-primary" Text="Submit" OnClick="btnChangeStatus_Click" />
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
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

        function showChangeStatus(id, name, status) {
            document.getElementById("<%=txtIdStatus.ClientID %>").value = id;
            document.getElementById("<%=txtName.ClientID %>").value = name;
            document.getElementById("<%=txtOldStatus.ClientID %>").value = status;
            document.getElementById("<%=ddlOldStatus.ClientID %>").value = status;

            let title = "";
            if (active === "1") {
                title = "Deactivate Fabric";
            } else {
                title = "Activate Fabric";
            }
            document.getElementById("titleActive").innerHTML = title;
        }

        function showLog(type, dataId) {
            $("#logError").addClass("d-none").html("");
            $("#tblLogs tbody").html("");
            $("#modalLog").modal("show");

            $.ajax({
                type: "POST",
                url: "/Setting/Method.aspx/GetLogs",
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

        ["modalChangeStatus", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>