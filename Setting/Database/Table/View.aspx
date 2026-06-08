<%@ Page Language="VB" AutoEventWireup="false" CodeFile="View.aspx.vb" Inherits="Setting_Database_Table_View" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="View Table" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3>Blind Type</h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/database">Database</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/database/table">Table</a></li>
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
                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-content">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-6">
                                            <h4 class="card-title" runat="server" id="hTitle"></h4>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                            <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Search Key : </span>
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
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="true" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Visible="false" OnPageIndexChanging="gvList_PageIndexChanging" OnDataBound="gvList_DataBound">
                                                </asp:GridView>
                                            </div>
                                            <div class="d-flex justify-content-end mt-3">
                                                <nav id="navPager" runat="server" visible="false">
                                                    <ul class="pagination pagination mb-0">
                                                        <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                                            <ItemTemplate>
                                                                <li class='page-item <%# Eval("CssClass") %>'>
                                                                    <asp:LinkButton runat="server" ID="lnkPage" CssClass="page-link" Text='<%# Eval("Text") %>' CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>' />
                                                                </li>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ul>
                                                </nav>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer">
                                <div class="col-12 form-group">
                                    <label class="form-label">Display</label>
                                    <asp:ListBox runat="server" ID="lblFieldName" CssClass="choices form-select multiple-remove" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="lblFieldName_SelectedIndexChanged"></asp:ListBox>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </section>
    </div>

    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblName"></asp:Label>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });

        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });

            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";

                initChoices();
            });
        }

        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });

        function initChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {

                if (el.choices) {
                    el.choices.destroy();
                }

                el.choices = new Choices(el, {
                    searchEnabled: true,
                    removeItemButton: true,
                    shouldSort: false,
                    searchResultLimit: 20,
                    renderChoiceLimit: -1,
                    placeholder: true,
                    placeholderValue: 'Select columns...',
                    itemSelectText: ''
                });

            });
        }

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>