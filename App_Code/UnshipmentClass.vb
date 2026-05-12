Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class UnshipmentClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetDataRow(thisString As String) As DataRow
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using thisAdapter As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        thisAdapter.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        Else
                            Return Nothing
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataRowSP(spName As String, params As List(Of SqlParameter)) As DataRow
        Try
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(spName, conn)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddRange(params.ToArray())

                    Using da As New SqlDataAdapter(cmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)

                        If dt.Rows.Count > 0 Then
                            Return dt.Rows(0)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return Nothing
    End Function

    Public Function GetDataTable(thisString As String) As DataTable
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As New SqlCommand(thisString, thisConn)
                    Using da As New SqlDataAdapter(thisCmd)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        Return dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetDataTableSP(spName As String, params As List(Of SqlParameter)) As DataTable
        Dim dt As New DataTable()
        Try
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(spName, conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    If params IsNot Nothing AndAlso params.Count > 0 Then
                        cmd.Parameters.AddRange(params.ToArray())
                    End If

                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            dt = New DataTable()
        End Try
        Return dt
    End Function

    Protected Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Function BindContent(searchText As String, companyId As String, roleName As String, levelName As String, loginId As String) As Byte()
        Using ms As New MemoryStream()
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@SearchText", searchText.Trim()),
                New SqlParameter("@CompanyId", If(String.IsNullOrEmpty(companyId), CType(DBNull.Value, Object), companyId)),
                 New SqlParameter("@RoleName", roleName),
                 New SqlParameter("@LevelName", levelName),
                 New SqlParameter("@LoginId", loginId)
            }

            Dim doc As New Document(PageSize.A4.Rotate, 20, 20, 80, 50)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, ms)

            writer.PageEvent = New UnshipmentEvents("UNSHIPMENT ORDER")
            doc.Open()

            Dim table As New PdfPTable(6)
            table.WidthPercentage = 100
            table.SetWidths(New Single() {0.1F, 0.3F, 0.2F, 0.2F, 0.1F, 0.1F})

            table.AddCell(CreateCell("ORDER ID", isBold:=True))
            table.AddCell(CreateCell("CUSTOMER NAME", isBold:=True))
            table.AddCell(CreateCell("ORDER NUMBER", isBold:=True))
            table.AddCell(CreateCell("ORDER NAME", isBold:=True))
            table.AddCell(CreateCell("PRODUCTION DATE", isBold:=True))
            table.AddCell(CreateCell("FACTORY", isBold:=True))

            Dim thisData As DataTable = GetDataTableSP("sp_OrderUnshipment", params)
            If thisData.Rows.Count > 0 Then
                For i As Integer = 0 To thisData.Rows.Count - 1
                    Dim id As String = thisData.Rows(i)("Id").ToString()
                    Dim orderId As String = thisData.Rows(i)("OrderId").ToString()
                    Dim customerName As String = thisData.Rows(i)("CustomerName").ToString()
                    Dim orderNumber As String = thisData.Rows(i)("OrderNumber").ToString()
                    Dim orderName As String = thisData.Rows(i)("OrderName").ToString()
                    Dim prodDate As String = If(IsDBNull(thisData.Rows(i)("ProductionDate")), "", Convert.ToDateTime(thisData.Rows(i)("ProductionDate")).ToString("dd MMM yyyy"))

                    Dim factoryList As New List(Of String)

                    Dim detailData As DataTable = GetDataTable("SELECT OrderDetails.*, Products.Name AS ProductName, Designs.Name AS DesignName, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Designs ON Products.DesignId=Designs.Id LEFT JOIN Blinds ON Products.DesignId=Blinds.Id WHERE OrderDetails.HeaderId='" & id & "'")
                    For iDetail As Integer = 0 To detailData.Rows.Count - 1
                        Dim designName As String = detailData.Rows(iDetail)("DesignName").ToString()
                        Dim blindName As String = detailData.Rows(iDetail)("BlindName").ToString()

                        Dim isBig As Boolean = False
                        Dim isAustralia As Boolean = False
                        Dim isTaiwan As Boolean = False
                        Dim isChina As Boolean = False

                        If designName = "Aluminium Blind" OrElse designName = "Design Shades" OrElse designName = "Linea Valance" OrElse designName = "Panel Glide" OrElse designName = "Pelmet" OrElse designName = "Roman Blind" OrElse designName = "Privacy Venetian" OrElse designName = "Venetian Blind" OrElse designName = "Vertical" OrElse designName = "Roller Blind" OrElse designName = "Sample" OrElse designName = "Skyline Shutter Express" OrElse designName = "Outdoor" OrElse designName = "Saphora Drape" OrElse designName = "Roller Horizon" Then
                            isBig = True
                        End If

                        If designName = "Cellular Shades" Then
                            Dim fabricColourId As String = detailData.Rows(iDetail)("FabricColourId").ToString()
                            Dim fabricFactory As String = GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourId & "'")

                            If fabricFactory = "Express" Then isBig = True
                            If fabricFactory = "Regular" Then isTaiwan = True
                        End If

                        If designName = "Curtain" Then
                            Dim fabricColourId As String = detailData.Rows(iDetail)("FabricColourId").ToString()
                            Dim fabricColourIdB As String = detailData.Rows(iDetail)("FabricColourIdB").ToString()

                            Dim trackType As String = detailData.Rows(iDetail)("TrackType").ToString()
                            Dim trackTypeB As String = detailData.Rows(iDetail)("TrackTypeB").ToString()

                            Dim fabricFactory As String = String.Empty
                            Dim fabricFactoryB As String = String.Empty

                            If fabricColourId <> "" Then
                                fabricFactory = GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourId & "'")
                            End If

                            If fabricColourIdB <> "" Then
                                fabricFactoryB = GetItemData("SELECT Factory FROM FabricColours WHERE Id='" & fabricColourIdB & "'")
                            End If

                            If fabricFactory = "Express" Then
                                isBig = True
                            ElseIf fabricFactory = "Regular" Then
                                isAustralia = True
                            End If

                            If fabricFactoryB = "Express" Then
                                isBig = True
                            ElseIf fabricFactoryB = "Regular" Then
                                isAustralia = True
                            End If

                            If trackType = "Standard Track" Then
                                isAustralia = True
                            ElseIf trackType = "Motorised Track" Then
                                isBig = True
                            End If

                            If trackTypeB = "Standard Track" Then
                                isAustralia = True
                            ElseIf trackTypeB = "Motorised Track" Then
                                isBig = True
                            End If
                        End If

                        If designName = "Skyline Shutter Ocean" OrElse designName = "Evolve Shutter Ocean" Then
                            isChina = True
                        End If

                        If designName = "Door" OrElse designName = "Window" Then
                            Dim frameColour As String = detailData.Rows(iDetail)("FrameColour").ToString()

                            If frameColour.Contains("Regular") Then isAustralia = True
                            If frameColour.Contains("Express") Then isBig = True
                        End If

                        If isBig AndAlso Not factoryList.Contains("BIG") Then
                            factoryList.Add("BIG")
                        End If

                        If isTaiwan AndAlso Not factoryList.Contains("TAIWAN") Then
                            factoryList.Add("TAIWAN")
                        End If

                        If isAustralia AndAlso Not factoryList.Contains("AUSTRALIA") Then
                            factoryList.Add("AUS")
                        End If

                        If isChina AndAlso Not factoryList.Contains("CHINA") Then
                            factoryList.Add("CHINA")
                        End If
                    Next

                    Dim factory As String = String.Join(", ", factoryList)

                    table.AddCell(CreateCell(orderId))
                    table.AddCell(CreateCell(customerName))
                    table.AddCell(CreateCell(orderNumber))
                    table.AddCell(CreateCell(orderName))
                    table.AddCell(CreateCell(prodDate))
                    table.AddCell(CreateCell(factory))
                Next
            End If

            doc.Add(table)
            doc.Close()

            Return ms.ToArray()
        End Using
    End Function

    Private Function CreateCell(text As String, Optional isBold As Boolean = False, Optional alignH As Integer = Element.ALIGN_LEFT) As PdfPCell
        If text Is Nothing Then text = String.Empty

        Dim normalFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, If(isBold, Font.BOLD, Font.NORMAL))
        Dim italicFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.ITALIC)

        Dim paragraph As New Paragraph()

        Dim currentIndex As Integer = 0

        While currentIndex < text.Length
            Dim startItalic As Integer = text.IndexOf("<i>", currentIndex)

            If startItalic = -1 Then
                paragraph.Add(New Chunk(text.Substring(currentIndex), normalFont))
                Exit While
            End If

            If startItalic > currentIndex Then
                paragraph.Add(New Chunk(text.Substring(currentIndex, startItalic - currentIndex), normalFont))
            End If

            Dim endItalic As Integer = text.IndexOf("</i>", startItalic)

            If endItalic = -1 Then
                paragraph.Add(New Chunk(text.Substring(startItalic), normalFont))
                Exit While
            End If

            Dim italicText As String = text.Substring(startItalic + 3, endItalic - (startItalic + 3))
            paragraph.Add(New Chunk(italicText, italicFont))

            currentIndex = endItalic + 4
        End While

        Dim cell As New PdfPCell(paragraph)

        cell.Border = Rectangle.BOX
        cell.BorderWidth = 0.5F

        cell.HorizontalAlignment = alignH
        cell.VerticalAlignment = Element.ALIGN_MIDDLE

        cell.PaddingTop = 4
        cell.PaddingBottom = 6
        cell.PaddingLeft = 4
        cell.PaddingRight = 4

        Return cell
    End Function
End Class

Public Class UnshipmentEvents
    Inherits PdfPageEventHelper

    Private pageTitle As String

    Public Sub New(pageTitle As String)
        Me.pageTitle = pageTitle
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)

        Dim cb As PdfContentByte = writer.DirectContent

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True
        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()
        Dim chunk1 As New Chunk(pageTitle.ToUpper(), New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD))
        phrase.Add(chunk1)

        Dim leftHeaderCell As New PdfPCell(phrase)
        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(leftHeaderCell)

        Dim rightHeaderCell As New PdfPCell(
            New Phrase(
                "Date : " & DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"),
                New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)
            )
        )

        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_BOTTOM
        headerTable.AddCell(rightHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 20, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True
        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(
            New Phrase(
                "All information within this report is private and confidential.",
                New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)
            )
        )

        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(
            New Phrase(
                "Page " & writer.PageNumber,
                New Font(Font.FontFamily.TIMES_ROMAN, 10)
            )
        )

        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 20, document.PageSize.GetBottom(36), cb)

    End Sub
End Class