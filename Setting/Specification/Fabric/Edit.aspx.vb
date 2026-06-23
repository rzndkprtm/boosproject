Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim returnPage As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("fabricid")) Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            returnPage = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("fabricid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub lbDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim designIds As New List(Of String)

        For Each item As ListItem In lbDesign.Items
            If item.Selected AndAlso Not String.IsNullOrEmpty(item.Value) Then
                designIds.Add(item.Value)
            End If
        Next

        If designIds.Count = 0 Then
            lbTube.Items.Clear()
            Exit Sub
        End If

        BindTube(designIds)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "FABRIC NAME IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim designType As String = String.Empty
                If Not lbDesign.SelectedValue = "" Then
                    Dim design As String = String.Empty
                    For Each item As ListItem In lbDesign.Items
                        If item.Selected Then
                            design += item.Value & ","
                        End If
                    Next
                    designType = design.Remove(design.Length - 1).ToString()
                End If
                Dim tubeType As String = String.Empty
                If Not lbTube.SelectedValue = "" Then
                    Dim tube As String = String.Empty
                    For Each item As ListItem In lbTube.Items
                        If item.Selected Then
                            tube += item.Value & ","
                        End If
                    Next
                    tubeType = tube.Remove(tube.Length - 1).ToString()
                End If
                Dim companyDetail As String = String.Empty
                If Not lbCompany.SelectedValue = "" Then
                    Dim company As String = String.Empty
                    For Each item As ListItem In lbCompany.Items
                        If item.Selected Then
                            company += item.Value & ","
                        End If
                    Next
                    companyDetail = company.Remove(company.Length - 1).ToString()
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Fabrics SET DesignId=@DesignId, TubeId=@TubeId, CompanyDetailId=@CompanyDetailId, Name=@Name, Type=@Type, [Group]=@Group, NoRailRoad=@NoRailRoad WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@DesignId", designType)
                        myCmd.Parameters.AddWithValue("@TubeId", tubeType)
                        myCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Group", ddlGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@NoRailRoad", ddlNoRailRoad.SelectedValue)
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Fabrics", lblId.Text, Session("LoginId").ToString(), "Fabric Type Updated"}
                settingClass.Logs(dataLog)

                url = "~/setting/specification/fabric/"
                If returnPage = "detail" Then
                    url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", lblId.Text)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/specification/fabric/"
        If returnPage = "detail" Then
            url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", lblId.Text)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(fabricId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Fabrics WHERE Id='" & fabricId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/specification/fabric", False)
                Exit Sub
            End If

            Dim designIds As New List(Of String)
            If thisData("DesignId").ToString() <> "" Then
                For Each id As String In thisData("DesignId").ToString().Split(","c)
                    id = id.Trim()
                    If id <> "" Then
                        designIds.Add(id)
                    End If
                Next
            End If

            BindDesign()
            BindTube(designIds)
            BindCompanyDetail()

            txtName.Text = thisData("Name").ToString()
            ddlType.SelectedValue = thisData("Type").ToString()
            ddlGroup.SelectedValue = thisData("Group").ToString()
            ddlNoRailRoad.SelectedValue = Convert.ToInt32(thisData("NoRailRoad"))

            If Not String.IsNullOrWhiteSpace(thisData("DesignId").ToString()) Then
                For Each i As String In thisData("DesignId").ToString().Split(","c)
                    Dim value As String = i.Trim()

                    If value <> "" Then
                        Dim item As ListItem = lbDesign.Items.FindByValue(value)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If

            If Not String.IsNullOrWhiteSpace(thisData("TubeId").ToString()) Then
                For Each i As String In thisData("TubeId").ToString().Split(","c)
                    Dim value As String = i.Trim()

                    If value <> "" Then
                        Dim item As ListItem = lbTube.Items.FindByValue(value)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If
            If Not String.IsNullOrWhiteSpace(thisData("CompanyDetailId").ToString()) Then
                For Each i As String In thisData("CompanyDetailId").ToString().Split(","c)
                    Dim value As String = i.Trim()

                    If value <> "" Then
                        Dim item As ListItem = lbCompany.Items.FindByValue(value)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS thisArray WHERE thisArray.VALUE='Fabrics' ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 0 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindTube(designIds As List(Of String))
        lbTube.Items.Clear()
        Try
            Dim conditions As New List(Of String)
            For Each designId As String In designIds

                Dim designName As String = settingClass.GetItemData("SELECT Name FROM Designs WHERE Id='" & designId.Replace("'", "''") & "'")

                If InStr(designName, "Roman", CompareMethod.Text) > 0 Then
                    conditions.Add("Alias LIKE '%(Roman)%'")
                End If
                If InStr(designName, "Panel Glide", CompareMethod.Text) > 0 Then
                    conditions.Add("Alias LIKE '%(PG)%'")
                End If
            Next

            If conditions.Count = 0 Then Exit Sub

            Dim query As String = "SELECT DISTINCT * FROM ProductTubes CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS applyArray WHERE applyArray.VALUE='Fabrics' AND (" & String.Join(" OR ", conditions.Distinct()) & ") ORDER BY Name ASC"

            lbTube.DataSource = settingClass.GetDataTable(query)
            lbTube.DataTextField = "Alias"
            lbTube.DataValueField = "Id"
            lbTube.DataBind()

            If lbTube.Items.Count > 0 Then
                lbTube.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        lbCompany.Items.Clear()
        Try
            lbCompany.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails ORDER BY Name ASC")
            lbCompany.DataTextField = "Name"
            lbCompany.DataValueField = "Id"
            lbCompany.DataBind()

            If lbCompany.Items.Count > 0 Then
                lbCompany.Items.Insert(0, New ListItem("", ""))
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

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
