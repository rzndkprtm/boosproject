<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Mailing.aspx.vb" Inherits="Setting_General_Mailing" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Mailing" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/general">General</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row mb-3">
            <div class="col-12 d-flex justify-content-end flex-wrap gap-2">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
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
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-6 mb-2"></div>
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
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="CompanyName" HeaderText="Company" />
                                                <asp:BoundField DataField="Name" HeaderText="Mail Name" />
                                                <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li runat="server" visible='<%# PageAction("Detail") %>'>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Copy") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalCopy" onclick='<%# String.Format("return showCopy(`{0}`);", Eval("Id").ToString()) %>'>Copy</a>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Delete") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                            </li>
                                                            <li>
                                                                <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('Mailings', '<%# Eval("Id") %>')">Log</a>
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
                    <div class="card-footer d-flex justify-content-between"></div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 runat="server" class="modal-title" id="titleProcess"></h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Company</label>
                            <asp:DropDownList runat="server" ID="ddlCompanyId" CssClass="form-select"></asp:DropDownList>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Mailing Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Server</label>
                            <asp:TextBox runat="server" ID="txtServer" CssClass="form-control" placeholder="Server ..." autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Host</label>
                            <asp:TextBox runat="server" ID="txtHost" CssClass="form-control" placeholder="Host ..." autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Port</label>
                            <asp:TextBox runat="server" ID="txtPort" CssClass="form-control" placeholder="Port ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Network Credentials</label>
                            <asp:DropDownList runat="server" ID="ddlNetworkCredentials" CssClass="form-select">
                                <asp:ListItem Value="0" Text="False"></asp:ListItem>
                                <asp:ListItem Value="1" Text="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Default Credentials</label>
                            <asp:DropDownList runat="server" ID="ddlDefaultCredentials" CssClass="form-select">
                                <asp:ListItem Value="0" Text="False"></asp:ListItem>
                                <asp:ListItem Value="1" Text="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Enable SSL</label>
                            <asp:DropDownList runat="server" ID="ddlEnableSSL" CssClass="form-select">
                                <asp:ListItem Value="0" Text="False"></asp:ListItem>
                                <asp:ListItem Value="1" Text="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Mail Account</label>
                            <asp:TextBox runat="server" ID="txtAccount" CssClass="form-control" placeholder="Mail Account ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Mail Password</label>
                            <asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" placeholder="Mail Password ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Mail Alias</label>
                            <asp:TextBox runat="server" ID="txtAlias" CssClass="form-control" placeholder="Mail Alias ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Mail Subject</label>
                            <asp:TextBox runat="server" ID="txtSubject" CssClass="form-control" placeholder="Mail Subject ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Mail TO</label>
                            <asp:TextBox runat="server" ID="txtTo" CssClass="form-control" placeholder="Mail To ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Mail CC</label>
                            <asp:TextBox runat="server" ID="txtCc" CssClass="form-control" placeholder="Mail CC ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Mail BCC</label>
                            <asp:TextBox runat="server" ID="txtBcc" CssClass="form-control" placeholder="Mail To ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                            <label class="form-label">Active</label>
                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcess"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcess" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalCopy" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Copy Mailing</h5>
                </div>

                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdCopy" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCopy" CssClass="btn btn-info" Text="Confirm" OnClick="btnCopy_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Mailing</h5>
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

        function showProcess() {
            $("#modalProcess").modal("show");
        }

        function showCopy(id) {
            document.getElementById("<%=txtIdCopy.ClientID %>").value = id;
        }

        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
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

        ["modalProcess", "modalCopy", "modalDelete", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>
