<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Setting_Customer_Login" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Login" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer">Customer</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row mb-3">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
            </div>
        </section>

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                    
                                </div>
                                <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                    <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                        <div class="input-group">
                                            <span class="input-group-text">Search : </span>
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder=""></asp:TextBox>
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
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                                <asp:BoundField DataField="RoleName" HeaderText="Role" />
                                                <asp:BoundField DataField="LevelName" HeaderText="Level" />
                                                <asp:BoundField DataField="UserName" HeaderText="User" />
                                                <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                                                <asp:BoundField DataField="LastLogin" HeaderText="Last Login" DataFormatString="{0:dd MMM yyyy HH:mm:ss}" />
                                                <asp:TemplateField ItemStyle-Width="120px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                        <ul class="dropdown-menu">
                                                            <li>
                                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkDetail" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalActive" onclick='<%# String.Format("return showActive(`{0}`, `{1}`);", Eval("Id").ToString(), Convert.ToInt32(Eval("Active"))) %>'><%# TextActive(Eval("Active")) %></a>
                                                            </li>
                                                            <li>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                            </li>
                                                            <li>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalResetPass" onclick='<%# String.Format("return showResetPass(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("UserName").ToString()) %>'>Reset Password</a>
                                                            </li>
                                                            <li>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDencryptPass" onclick='<%# String.Format("return showDencryptPass(`{0}`, `{1}`);", Eval("UserName").ToString(), DencryptPassword(Eval("Password").ToString())) %>'>Show Password</a>
                                                            </li>
                                                            <li><hr class="dropdown-divider"></li>
                                                            <li>
                                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLog" Text="Log" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
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

    <div class="modal modal-blur fade" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleProcess"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcess"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Customer Account</label>
                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Role</label>
                            <asp:DropDownList runat="server" ID="ddlRole" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-6 form-group">
                            <label class="form-label">Level</label>
                            <asp:DropDownList runat="server" ID="ddlLevel" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Full Name</label>
                            <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" placeholder="Full Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Email</label>
                            <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Username</label>
                            <asp:TextBox runat="server" ID="txtUserName" CssClass="form-control" placeholder="UserName ..." autocomplete="new-password"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Password</label>
                            <asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" placeholder="Password ..."></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-3 form-group">
                            <label class="form-label">Pricing</label>
                            <asp:DropDownList runat="server" ID="ddlPricing" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcess" Text="Submit" CssClass="btn btn-primary" OnClick="btnProcess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalActive" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white" id="titleActive"></h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdActive" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtActive" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnActive" CssClass="btn btn-warning" Text="Confirm" OnClick="btnActive_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Login</h5>
                </div>
                <div class="modal-body text-center py-4">
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

    <div class="modal modal-blur fade" id="modalResetPass" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Reset Password</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdResetPass" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtNewResetPass" style="display:none;"></asp:TextBox>
                    <span id="spanDescResetPass"></span>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnResetPass" CssClass="btn btn-info" Text="Confirm" OnClick="btnResetPass_Click" />
                </div>
            </div>
        </div>
    </div>    

    <div class="modal modal-blur fade" id="modalDencryptPass" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white">Show Password</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <span id="spanPassword"></span>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
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
                        <asp:GridView runat="server" ID="gvListLogs" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeader="false" GridLines="None" BorderStyle="None">
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
        function showProcess() {
            $("#modalProcess").modal("show");
        }

        function showActive(id, active) {
            document.getElementById("<%=txtIdActive.ClientID %>").value = id;
            document.getElementById("<%=txtActive.ClientID %>").value = active;

            let title = "";
            if (active === "1") {
                title = "Disable Login";
            } else {
                title = "Enable Login";
            }
            document.getElementById("titleActive").innerHTML = title;
        }

        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }

        function showLog() {
            $("#modalLog").modal("show");
        }

        function showResetPass(id, username) {
            let newPass = generateNewPassword(15);
            let result = `Hi <b><%: Session("FullName") %></b>,<br />Are you sure you want to reset this account password?<br /><br /><b>USERNAME : ${username.toUpperCase()}</b><br /><b>USER ID : ${id.toUpperCase()}</b><br/><br />NEW PASSWORD : <br/><b>${newPass}</b>`;

            document.getElementById("<%=txtIdResetPass.ClientID %>").value = id;
            document.getElementById("<%=txtNewResetPass.ClientID %>").value = newPass;
            document.getElementById("spanDescResetPass").innerHTML = result;
        }

        function generateNewPassword(length) {
            const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            let result = "";
            const cryptoArray = new Uint8Array(length);
            window.crypto.getRandomValues(cryptoArray);
            for (let i = 0; i < length; i++) {
                result += chars[cryptoArray[i] % chars.length];
            }
            return result;
        }

        function showDencryptPass(username, password) {
            let body = "UserName";
            body += "<br />";
            body += "<b>" + username + "</b>";
            body += "<br /><br />";
            body += "Password Decryption";
            body += "<br />";
            body += "<b>" + password + "</b>";
            document.getElementById("spanPassword").innerHTML = body;
        }

        ["modalProcess", "modalActive", "modalDelete", "modalResetPass", "modalDencryptPass", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) return;

            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';

                row.addEventListener('click', function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                        return;
                    }

                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                });
            }
        });

        window.history.replaceState(null, null, window.location.href);
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
        <asp:Label runat="server" ID="lblUserName"></asp:Label>
    </div>
</asp:Content>
