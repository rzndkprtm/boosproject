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
            BindCompanyDetail()
            ddlCompanyDetail.SelectedValue = Session("CompanyFabric")
            BindData(txtSearch.Text, ddlCompanyDetail.SelectedValue)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/fabric/add", False)
    End Sub

    Protected Sub btnAlias_Click(sender As Object, e As EventArgs)
        Session("SearchFabric") = txtSearch.Text
        Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
        Response.Redirect("~/setting/specification/fabric/alias", False)
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
            Session("CompanyFabric") = ddlCompanyDetail.SelectedValue

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

            Dim aliasData As DataTable = settingClass.GetDataTable("SELECT * FROM FabricAlias WHERE Type='Fabrics' AND FirstId='" & thisId & "'")
            If aliasData.Rows.Count > 0 Then
                For i As Integer = 0 To aliasData.Rows.Count - 1
                    Dim aliasId As String = aliasData.Rows(i)("SecondId").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET Active=@Active WHERE Id=@Id", thisConn)
                            myCmd.Parameters.AddWithValue("@Id", aliasId)
                            myCmd.Parameters.AddWithValue("@Active", active)

                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                        End Using
                    End Using

                    dataLog = {"Fabrics", aliasId, Session("LoginId").ToString(), activeDesc}
                    settingClass.Logs(dataLog)
                Next
            End If

            Session("SearchFabric") = txtSearch.Text
            Session("CompanyFabric") = ddlCompanyDetail.SelectedValue
            Response.Redirect("~/setting/specification/fabric", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(searchText As String, companyText As String)
        Session("SearchFabric") = String.Empty
        Session("CompanyFabric") = String.Empty
        Try
            Dim conditions As New List(Of String)
            Dim thisArray As String = String.Empty

            If Not String.IsNullOrEmpty(companyText) Then
                thisArray = "CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS cdArray"
                conditions.Add("cdArray.VALUE = '" & companyText & "'")
            End If

            If Not String.IsNullOrEmpty(searchText) Then
                conditions.Add("(Id LIKE '%" & searchText & "%' OR Name LIKE '%" & searchText & "%')")
            End If

            Dim whereClause As String = ""
            If conditions.Count > 0 Then
                whereClause = "WHERE " & String.Join(" AND ", conditions)
            End If

            Dim thisString As String = String.Format("SELECT *, CASE WHEN Active=1 THEN 'Yes' WHEN Active=0 THEN 'No' ELSE 'Error' END AS DataActive FROM Fabrics {0} {1} ORDER BY Name ASC", thisArray, whereClause)

            gvList.DataSource = settingClass.GetDataTable(thisString)
            gvList.DataBind()
            gvList.Columns(1).Visible = PageAction("Visible ID")
            gvList.Columns(5).Visible = PageAction("Visible Company Detail")

            btnAdd.Visible = PageAction("Add")
            btnAlias.Visible = PageAction("Visible Fabric Alias")

            divCompanyDetail.Visible = PageAction("Visible Sort Company Detail")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        ddlCompanyDetail.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM CompanyDetails ORDER BY Id ASC"
            If Not Session("CompanyDetailId") = "" Then
                thisString = "SELECT * FROM CompanyDetails WHERE CompanyId='" & Session("CompanyId") & "' ORDER BY Id ASC"
            End If
            ddlCompanyDetail.DataSource = settingClass.GetDataTable(thisString)
            ddlCompanyDetail.DataTextField = "Name"
            ddlCompanyDetail.DataValueField = "Id"
            ddlCompanyDetail.DataBind()

            If ddlCompanyDetail.Items.Count > 0 Then
                ddlCompanyDetail.Items.Insert(0, New ListItem("", ""))
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
                    Dim companyDetailName As String = myData.Rows(i)("CompanyName").ToString()
                    hasil += companyDetailName & ", "
                Next
                Return hasil.Remove(hasil.Length - 2).ToString()
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
