Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Edit
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/fabric", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDesign()
            BindTube()
            BindCompanyDetail()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "FABRIC NAME IS REQUIRED !")
                Exit Sub
            End If

            Dim designSelected As String = String.Empty
            Dim tubeSelected As String = String.Empty
            Dim companySelected As String = String.Empty

            For Each item As ListItem In lbDesign.Items
                If item.Selected Then
                    designSelected += item.Value & ","
                End If
            Next
            If designSelected = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            For Each item As ListItem In lbCompany.Items
                If item.Selected Then
                    companySelected += item.Value & ","
                End If
            Next
            If companySelected = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim designType As String = designSelected.Remove(designSelected.Length - 1).ToString()
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
                Dim companyDetail As String = companySelected.Remove(companySelected.Length - 1).ToString()

                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Fabrics ORDER BY Id DESC")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Fabrics VALUES (@Id, @DesignId, @TubeId, @CompanyId, @Name, @Type, @Group, @NoRailRoad, @Active)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@DesignId", designType)
                        myCmd.Parameters.AddWithValue("@TubeId", tubeType)
                        myCmd.Parameters.AddWithValue("@CompanyId", companyDetail)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Type", ddlType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Group", ddlGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@NoRailRoad", ddlNoRailRoad.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Fabrics", thisId, Session("LoginId").ToString(), "Fabric Created"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", thisId)
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
        Response.Redirect("~/setting/specification/fabric", False)
    End Sub

    Protected Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            Dim thisString As String = "SELECT * FROM Designs CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS thisArray WHERE thisArray.VALUE='Fabrics' AND Active=1 ORDER BY Name ASC"
            lbDesign.DataSource = settingClass.GetDataTable(thisString)
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

    Protected Sub BindTube()
        lbTube.Items.Clear()
        Try
            lbTube.DataSource = settingClass.GetDataTable("SELECT * FROM ProductTubes ORDER BY Name ASC")
            lbTube.DataTextField = "Name"
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
            Dim thisString As String = "SELECT * FROM CompanyDetails WHERE Active=1 ORDER BY Name ASC"
            lbCompany.DataSource = settingClass.GetDataTable(thisString)
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
