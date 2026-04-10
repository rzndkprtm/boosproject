<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Stocks.aspx.vb" Inherits="Stocks" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Stocks" %>

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
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                <a class="list-group-item list-group-item-action active" id="listRoller" data-bs-toggle="list" href="#list-roller" role="tab">Roller</a>

                                <a class="list-group-item list-group-item-action" id="listProfile" data-bs-toggle="list" href="#list-profile" role="tab">Design Shades</a>

                                <a class="list-group-item list-group-item-action" id="listCurtain" data-bs-toggle="list" href="#list-curtain" role="tab">Curtain</a>

                                <a class="list-group-item list-group-item-action" id="listVertical" data-bs-toggle="list" href="#list-vertical" role="tab">Vertical</a>

                                <a class="list-group-item list-group-item-action" id="listVenetian" data-bs-toggle="list" href="#list-venetian" role="tab">Venetian</a>

                                <a class="list-group-item list-group-item-action" id="listAluminium" data-bs-toggle="list" href="#list-aluminium" role="tab">Aluminium</a>
                            </div>
                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-roller" role="tabpanel" aria-labelledby="listRoller">
                                    <div class="row mt-5" runat="server" id="divErrorRoller">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorRoller"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5" runat="server">
                                        <div class="col-12 col-sm-12 col-lg-7">
                                            <asp:Panel runat="server" DefaultButton="btnSearchRoller" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Fabric Type : </span>
                                                    <asp:TextBox runat="server" ID="txtSearchRoller" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearchRoller" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchRoller_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListRoller" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListRoller_RowDataBound">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-profile" role="tabpanel" aria-labelledby="listProfile">
                                    <div class="row mt-5" runat="server" id="divErrorProfile">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorProfile"></span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListProfile" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListProfile_RowDataBound">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-curtain" role="tabpanel" aria-labelledby="listCurtain">
                                    <div class="row mt-5" runat="server" id="divErrorCurtain">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorCurtain"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5" runat="server">
                                        <div class="col-12 col-sm-12 col-lg-7">
                                            <asp:Panel runat="server" DefaultButton="btnSearchCurtain" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Fabric Type : </span>
                                                    <asp:TextBox runat="server" ID="txtSearchCurtain" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearchCurtain" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchCurtain_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListCurtain" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListCurtain_RowDataBound">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-vertical" role="tabpanel" aria-labelledby="listVertical">
                                    <div class="row mt-5" runat="server" id="divErrorVertical">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorVertical"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5" runat="server">
                                        <div class="col-12 col-sm-12 col-lg-7">
                                            <asp:Panel runat="server" DefaultButton="btnSearchVertical" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Fabric Type : </span>
                                                    <asp:TextBox runat="server" ID="txtSearchVertical" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearchVertical" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchVertical_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListVertical" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListVertical_RowDataBound">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-venetian" role="tabpanel" aria-labelledby="listVenetian">
                                    <div class="row mt-5" runat="server" id="divErrorVenetian">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorVenetian"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListVenetian" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListVenetian_RowDataBound">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:BoundField DataField="BlindName" HeaderText="" />
                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-aluminium" role="tabpanel" aria-labelledby="listAluminium">
                                    <div class="row mt-5" runat="server" id="divErrorAluminium">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorAluminium"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListAluminium" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListAluminium_RowDataBound">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:BoundField DataField="BlindName" HeaderText="" />
                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <asp:HiddenField ID="selected_tab" runat="server" />

    <script type="text/javascript">
        $(document).ready(function () {
            var selectedTab = $("#<%=selected_tab.ClientID%>");
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "list-roller";
            $('#dvTab a[href="#' + tabId + '"]').tab('show');
            $("#dvTab a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });

            $("#listRoller").on("click", function () {
                updateSessionValue("list-roller");
            });
            $("#listProfile").on("click", function () {
                updateSessionValue("list-profile");
            });
            $("#listCurtain").on("click", function () {
                updateSessionValue("list-curtain");
            });
            $("#listVertical").on("click", function () {
                updateSessionValue("list-vertical");
            });
            $("#listVenetian").on("click", function () {
                updateSessionValue("list-venetian");
            });
            $("#listAluminium").on("click", function () {
                updateSessionValue("list-aluminium");
            });
        });

        function updateSessionValue(session) {
            $.ajax({
                type: "POST",
                url: "Detail.aspx/UpdateSession",
                data: JSON.stringify({ value: session }),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
        }

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
