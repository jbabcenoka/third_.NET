   M S B A M L   `   `   `  ����@  <DB_UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:DB_UIDB_UI  5   	       DB_UI.UpdateOrder   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:DB_UI     5   	     � Title$  UpdateOrder��5	   	   $	��450��6   $	��800��6*   .��5
      �  � ColumnDefinitions 5   
   �� 5      6!   �� 5      6!   �� 5      6!    � RowDefinitions 5   
   �� 5      6   �� 5      6   �� 5      6   .+�5   
   �� $ ��Pasūtījuma rediģēšana��6   $��0��60   $��1��6=   $��20(�6N   $��Center=�6\   $��39��6y   $��Center=�6�   $	��224��6�   �� 5   
   $��Sakotnējās vērtības��6   $��0��6/   $��0��6<   $��18(�6M   $��Center=�6[   $��30��6x   $
��Wrap=�6�   $��Bottom=�6�   $	��165��6�   �� 5   
   -   5         ��Name$ 	VecieDati��5      $��1��6'   $��0��64   $	��140��6D   $	��250��6R   $��Center=�6^   $
��Wrap=�6{   $	��Top=�6�   �� 5   
   -   5         ��Name$ IzvelejatiesKoMainit��5      $��1��6.   $��1��6;   $	��270��6K   $��Center=�6W   $ ��Izvēlējieties ko mainīt��6{   $��16(�6�   $��Center=�6�   $	��Top=�6�   �� 5   
   -   5         ��Name$ cboUpdateChoise��5      $��30��6,   $	��200��68   $��1��6E   $��1��6R   $��Center=�6b   $��Center=�6}   �� 5   
   -   5         ��Name$ Mainit��5      $��	Rediģēt��6!   $��1��63   $��1��6A   $��Center=�6Q   $��Bottom=�6o   $��16(�6�   $��27��6�   $	��150��6�   6�   �� 5   
   -   5        $ MainisanasTeksts��5      $y�Hidden=�6*   $��15(�6?   $��1��6M   $��Center=�6Z   $��1��6~   $��Center=�6�   $	��Top=�6�   $��30��6�   $	��250��6�   �� 5   
   -   5         ��Name$ PasutNumTxt��5      $y�Hidden=�6'   $��1��6<   $��1��6J   $��Center=�6\   $��Center=�6y   $��25��6�    �� TextWrapping$
 Wrap=�6�   $	��250��6�   �� 5    
   -   5        $ cboStavoklis��5       $y�Hidden=�6)   $��30��6?   $	��150��6K   $��1��6W   $��1��6d   $��Center=�6v   $��Center=�6�   (  "System.Windows.Controls.DatePicker  5!   
   -   5        	  Name$	 PasutijumaDatums��5!      $y�Hidden=�6/   $��30��6C   $	��150��6O   $��1��6[   $��1��6h   $��Center=�6z   $��Center=�6�   �� 5"   
   -	   5        $ cboPasutitajs��5"      $y�Hidden=�6*   $��30��6?   $	��150��6K   $��1��6W   $��1��6d   $��Center=�6v   $��Center=�6�   �� 5$   
   -
   5        $ RedigetPasutijumu��5$      $y�Hidden=�6,   $��	Saglabāt��6@   $��18(�6S   $��30��6a   $��1��6n   $��1��6{   $	��100��6�   $��Bottom=�6�   $
��Left=�6�   ���	#FFA5CDDC6�   �� 5%   
   -   5        $ 
AtpakalBtn��5%      $y�Hidden=�6%   $��Aptakaļ��69   $��18(�6K   $��30��6Y   $��1��6f   $��1��6s   $	��100��6�   $��Bottom=�6�   $��Right=�6�   ���	#FFA5CDDC6�   5&      5'      