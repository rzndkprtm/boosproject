<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Stocks.aspx.vb" Inherits="Stocks" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Stocks" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .grid-container { width: 100%; height: calc(100vh - 150px); overflow: auto; border: 1px solid #ddd; }
        .grid-container table { width: 100%; border-collapse: collapse; table-layout: auto; min-height: 180%; }
        .grid-container td, .grid-container th { white-space: nowrap; padding: 6px 10px; }
        .grid-container th { position: sticky; top: 0; background: #f5f5f5; z-index: 3; }
        .grid-container td:first-child, .grid-container th:first-child { position: sticky; left: 0; background: #fff; z-index: 2; white-space: normal; word-break: break-word; min-width: 220px; max-width: 320px; }
        .grid-container th:first-child { z-index: 5; background: #f5f5f5; }
        .grid-container tr:last-child td { position: sticky; bottom: 0; background: #f5f5f5; z-index: 3; }
        .grid-container tr:last-child td:first-child { left: 0; z-index: 6; background: #f5f5f5; }
    </style>
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
                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-content">
                                <div class="card-body">
                                    <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                        <a class="list-group-item list-group-item-action active" id="listRoller" data-bs-toggle="list" href="#list-roller" role="tab">Roller</a>
                                        <a class="list-group-item list-group-item-action" id="listVertical" data-bs-toggle="list" href="#list-vertical" role="tab">Vertical</a>
                                        <a class="list-group-item list-group-item-action" id="listCellular" data-bs-toggle="list" href="#list-cellular" role="tab">Cellular Shades</a>
                                        <a class="list-group-item list-group-item-action" id="listProfile" data-bs-toggle="list" href="#list-profile" role="tab">Design Shades</a>
                                        <a class="list-group-item list-group-item-action" id="listCurtain" data-bs-toggle="list" href="#list-curtain" role="tab">Curtain</a>
                                        <a class="list-group-item list-group-item-action" id="listVenetian" data-bs-toggle="list" href="#list-venetian" role="tab">Venetian Blind</a>
                                        <a class="list-group-item list-group-item-action" id="listAluminium" data-bs-toggle="list" href="#list-aluminium" role="tab">Aluminium Blind</a>
                                        <a class="list-group-item list-group-item-action" id="listFabricChart" data-bs-toggle="list" href="#list-fabricchart" role="tab">Fabric Chart</a>
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
                                            <div class="row mt-5">
                                                <div class="col-12">
                                                    <div class="table-responsive">
                                                        <table class="table mb-0">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                    <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                    <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                    <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListRoller" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListRoller_RowDataBound">
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
                                                        <table class="table mb-0">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                    <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                    <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                    <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListProfile" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListProfile_RowDataBound">
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
                                            <div class="row mt-5">
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
                                            <div class="row mt-5">
                                                <div class="col-12">
                                                    <div class="table-responsive">
                                                        <table class="table mb-0">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                    <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                    <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                    <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListCurtain" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListCurtain_RowDataBound">
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
                                            <div class="row mt-5">
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
                                            <div class="row mt-5">
                                                <div class="col-12">
                                                    <div class="table-responsive">
                                                        <table class="table mb-0">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                    <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                    <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                    <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListVertical" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListVertical_RowDataBound">
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
                                                        <table class="table mb-0">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                    <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                    <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                    <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListVenetian" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListVenetian_RowDataBound">
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
                                                        <table class="table mb-0">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                    <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                    <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                    <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListAluminium" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListAluminium_RowDataBound">
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
                                        <div class="tab-pane fade" id="list-cellular" role="tabpanel" aria-labelledby="listCellular">
                                            <div class="row mt-5" runat="server" id="divErrorCellular">
                                                <div class="col-12">
                                                    <div class="alert alert-danger">
                                                        <span runat="server" id="msgErrorCellular"></span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-5">
                                                <div class="col-12">
                                                    <div class="table-responsive">
                                                        <table class="table mb-0">
                                                            <tbody>
                                                                <tr>
                                                                    <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                    <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                    <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                    <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListCellular" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListCellular_RowDataBound">
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
                                        <div class="tab-pane fade" id="list-fabricchart" role="tabpanel" aria-labelledby="listFabricChart">
                                            <div class="row mt-5" runat="server" id="divErrorFabricChart">
                                                <div class="col-12">
                                                    <div class="alert alert-danger">
                                                        <span runat="server" id="msgErrorFabricChart"></span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mt-5" runat="server">
                                                <div class="col-12 col-sm-12 col-lg-7">
                                                    <asp:Panel runat="server" DefaultButton="btnFabricChart" Width="100%">
                                                        <div class="input-group">
                                                            <span class="input-group-text">Fabric Type : </span>
                                                            <asp:TextBox runat="server" ID="txtSearchFabricChart" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                            <asp:Button runat="server" ID="btnFabricChart" CssClass="btn btn-primary" Text="Search" OnClick="btnFabricChart_Click" />
                                                        </div>
                                                    </asp:Panel>
                                                </div>
                                            </div>
                                            <div class="row mt-3">
                                                <div class="col-12">
                                                    <div class="table-responsive grid-container">
                                                        <asp:GridView runat="server" ID="gvListFabricChart" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListFabricChart_RowDataBound">
                                                            <RowStyle />
                                                            <Columns>
                                                                <asp:BoundField DataField="FabricName" HeaderText="Fabric Type" />
                                                                <asp:BoundField DataField="Roller" HeaderText="Roller" />
                                                                <asp:BoundField DataField="RomanClassic" HeaderText="Roman (Classic)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                                <asp:BoundField DataField="RomanPlantation" HeaderText="Roman (Plantation)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                                <asp:BoundField DataField="RomanSewless" HeaderText="Roman (Sewless)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                                <asp:BoundField DataField="PGPlain" HeaderText="PG (Plain)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                                <asp:BoundField DataField="PGPlantation" HeaderText="PG (Plantation)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                                <asp:BoundField DataField="PGSewless" HeaderText="PG (Sewless)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                                <asp:BoundField DataField="Vertical89mm" HeaderText="Vertical (89mm)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                                <asp:BoundField DataField="Vertical127mm" HeaderText="Vertical (127mm)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer" runat="server" id="divCompanyDetail">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-4 d-flex justify-content-end">
                                        <div class="input-group">
                                            <span class="input-group-text">Company Detail</span>
                                            <asp:DropDownList runat="server" ID="ddlCompanyDetail" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompanyDetail_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
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

    <asp:HiddenField ID="selected_tab" runat="server" />

    <script type="text/javascript">
        function activateCurrentTab() {
            var tabId = $("#<%= selected_tab.ClientID %>").val();
            if (!tabId) tabId = "list-roller";
            $('#dvTab a[href="#' + tabId + '"]').tab('show');
        }

        function pageInit() {
            $(document).off("click.stocktab").on("click.stocktab", "#dvTab a", function () {
                var tabId = $(this).attr("href").replace("#", "");
                $("#<%= selected_tab.ClientID %>").val(tabId);
            });
            activateCurrentTab();
        }

        $(document).ready(function () {
            pageInit();
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            pageInit();
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
