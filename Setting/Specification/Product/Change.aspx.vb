Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Change
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDesignType()
            BindBlindType(ddlDesignType.SelectedValue)
            BindTubeType()
            BindControlType()
            BindColourType()
        End If
    End Sub

    Protected Sub ddlDesignType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlindType(ddlDesignType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesignType.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            If ddlStatus.SelectedValue = "" Then
                MessageError(True, "STATUS IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim blindString As String = String.Empty
                If Not ddlBlindType.SelectedValue = "" Then blindString = "AND BlindId='" & ddlBlindType.SelectedValue & "'"
                Dim tubeString As String = String.Empty
                If Not ddlTubeType.SelectedValue = "" Then tubeString = "AND TubeType='" & ddlTubeType.SelectedValue & "'"
                Dim controlString As String = String.Empty
                If Not ddlControlType.SelectedValue = "" Then controlString = "AND ControlType='" & ddlControlType.SelectedValue & "'"
                Dim colourString As String = String.Empty
                If Not ddlColourType.SelectedValue = "" Then colourString = "AND ColourType='" & ddlColourType.SelectedValue & "'"

                Dim thisString As String = String.Format("SELECT DISTINCT * FROM Products WHERE DesignId={0} AND (Status='In Stock' OR Status='Limited Stock' OR Status='Out of Stock' OR Status='Discontinued') {1} {2} {3} {4} ORDER BY Id ASC", ddlDesignType.SelectedValue, blindString, tubeString, controlString, colourString)
                Dim thisData As DataTable = settingClass.GetDataTable(thisString)

                If Not thisData.Rows.Count = 0 Then
                    For i As Integer = 0 To thisData.Rows.Count - 1
                        Dim productId As String = thisData.Rows(i)("Id").ToString()

                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("UPDATE Products SET Status=@Status WHERE Id=@Id", thisConn)
                                myCmd.Parameters.AddWithValue("@Id", productId)
                                myCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)

                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        Dim changeDesc As String = String.Format("Change Status Product : {0}", ddlStatus.SelectedValue)
                        dataLog = {"Products", productId, Session("LoginId").ToString(), changeDesc}
                        settingClass.Logs(dataLog)

                        Dim aliasData As DataRow = settingClass.GetDataRow("SELECT SecondId FROM ProductAlias WHERE FirstId='" & productId & "'")
                        If aliasData IsNot Nothing Then
                            Dim aliasId As String = aliasData(0).ToString()

                            Using thisConn As New SqlConnection(myConn)
                                Using myCmd As SqlCommand = New SqlCommand("UPDATE Products SET Status=@Status WHERE Id=@Id", thisConn)
                                    myCmd.Parameters.AddWithValue("@Id", aliasId)
                                    myCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)

                                    thisConn.Open()
                                    myCmd.ExecuteNonQuery()
                                End Using
                            End Using

                            changeDesc = String.Format("Change Status Product : {0}", ddlStatus.SelectedValue)
                            dataLog = {"Products", aliasId, Session("LoginId").ToString(), changeDesc}
                            settingClass.Logs(dataLog)
                        End If
                    Next

                    Response.Redirect("~/setting/specification/product", False)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/product", False)
    End Sub

    Protected Sub BindDesignType()
        ddlDesignType.Items.Clear()
        Try
            ddlDesignType.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesignType.DataTextField = "Name"
            ddlDesignType.DataValueField = "Id"
            ddlDesignType.DataBind()

            If ddlDesignType.Items.Count > 1 Then
                ddlDesignType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindBlindType(designId As String)
        ddlBlindType.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                ddlBlindType.DataSource = settingClass.GetDataTable("SELECT * FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlindType.DataTextField = "Name"
                ddlBlindType.DataValueField = "Id"
                ddlBlindType.DataBind()

                If ddlBlindType.Items.Count > 1 Then
                    ddlBlindType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindTubeType()
        ddlTubeType.Items.Clear()
        Try
            ddlTubeType.DataSource = settingClass.GetDataTable("SELECT * FROM ProductTubes ORDER BY Name ASC")
            ddlTubeType.DataTextField = "Name"
            ddlTubeType.DataValueField = "Id"
            ddlTubeType.DataBind()

            If ddlTubeType.Items.Count > 1 Then
                ddlTubeType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindControlType()
        ddlControlType.Items.Clear()
        Try
            ddlControlType.DataSource = settingClass.GetDataTable("SELECT * FROM ProductControls ORDER BY Name ASC")
            ddlControlType.DataTextField = "Name"
            ddlControlType.DataValueField = "Id"
            ddlControlType.DataBind()

            If ddlControlType.Items.Count > 1 Then
                ddlControlType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindColourType()
        ddlColourType.Items.Clear()
        Try
            ddlColourType.DataSource = settingClass.GetDataTable("SELECT * FROM ProductColours ORDER BY Name ASC")
            ddlColourType.DataTextField = "Name"
            ddlColourType.DataValueField = "Id"
            ddlColourType.DataBind()

            If ddlColourType.Items.Count > 1 Then
                ddlColourType.Items.Insert(0, New ListItem("", ""))
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
