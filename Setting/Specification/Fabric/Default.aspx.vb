Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Default
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            txtSearch.Text = Session("SearchFabric")

            BindCompanyDetailSort()
            BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/fabric/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub ddlCompanyDetail_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("SearchFabric") = txtSearch.Text

            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", dataId)
                    Response.Redirect(url, False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Developer" Then
                        MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
                    End If
                End Try
            End If
        End If
    End Sub

    Protected Sub btnActive_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtIdActive.Text

            Dim active As Integer = 1
            If txtActive.Text = "1" Then : active = 0 : End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Active=@Active WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", thisId)
                    myCmd.Parameters.AddWithValue("@Active", active)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim activeDesc As String = "Fabric Has Been Activated"
            If active = 0 Then activeDesc = "Fabric Has Been Deactivated"

            Dim dataLog As Object() = {"Fabrics", thisId, Session("LoginId").ToString(), activeDesc}
            settingClass.Logs(dataLog)

            Dim fabricDetail As DataTable = settingClass.GetDataTable("SELECT * FROM FabricColours WHERE FabricId='" & thisId & "'")
            If fabricDetail.Rows.Count > 0 Then
                For i As Integer = 0 To fabricDetail.Rows.Count - 1
                    Dim detailId As String = fabricDetail.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", detailId)
                            myCmd.Parameters.AddWithValue("@Active", active)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    activeDesc = "Fabric Colour Has Been Activated from Fabric Type"
                    If active = 0 Then activeDesc = "Fabric Colour Has Been Deactivated from Fabric Type"

                    dataLog = {"FabricColours", detailId, Session("LoginId").ToString(), activeDesc}
                    settingClass.Logs(dataLog)
                Next
            End If

            Session("SearchFabric") = txtSearch.Text
            Response.Redirect("~/setting/specification/fabric", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String, sortText As String)
        Session("SearchFabric") = String.Empty
        Try
            Dim sort As String = "WHERE EXISTS (SELECT 1 FROM STRING_SPLIT(Fabrics.CompanyDetailId, ',') companyArray WHERE LTRIM(RTRIM(companyArray.value)) = '" & sortText & "')"
            Dim byRole As String = String.Empty
            Dim search As String = String.Empty
            If Not searchText = "" Then
                search = "AND (Id LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%')"
            End If

            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Fabrics {0} {1} ORDER BY Id ASC", sort, search)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")
            gvList.Columns(5).Visible = PageAction("Visible Company Detail")

            btnAdd.Visible = PageAction("Add")
            aFabricAlias.Visible = PageAction("Visible Fabric Alias")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetailSort()
        ddlCompanyDetail.Items.Clear()
        Try
            If Not Session("CompanyDetailId") = "" Then
                Dim thisString As String = "SELECT * FROM CompanyDetails ORDER BY Id ASC"
                If Session("RoleName") = "Account" OrElse Session("RoleName") = "Customer Service" Then
                    thisString = "SELECT * FROM CompanyDetails WHERE CompanyId='" & Session("CompanyId") & "' ORDER BY Id ASC"
                End If

                ddlCompanyDetail.DataSource = settingClass.GetDataTable(thisString)
                ddlCompanyDetail.DataTextField = "Name"
                ddlCompanyDetail.DataValueField = "Id"
                ddlCompanyDetail.DataBind()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function BindCompany(fabricId As String) As String
        If Not String.IsNullOrEmpty(fabricId) Then
            Dim myData As DataTable = settingClass.GetDataTable("SELECT CompanyDetails.Name AS CompanyName FROM Fabrics CROSS APPLY STRING_SPLIT(Fabrics.CompanyDetailId, ',') AS splitArray LEFT JOIN CompanyDetails ON splitArray.VALUE=CompanyDetails.Id WHERE Fabrics.Id='" & fabricId & "' ORDER BY CompanyDetails.Id ASC")
            Dim hasil As String = String.Empty
            If myData.Rows.Count > 0 Then
                For i As Integer = 0 To myData.Rows.Count - 1
                    Dim designName As String = myData.Rows(i)("CompanyName").ToString()
                    hasil += designName & ","
                Next
                Return hasil.Remove(hasil.Length - 1).ToString()
            Else
                Return String.Empty
            End If
        End If
        Return "Error"
    End Function

    Protected Function TextActive(active As Boolean) As String
        If active = True Then Return "Deactivate"
        Return "Activate"
    End Function

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
