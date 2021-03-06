﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct F_Matrix {

    private int[,] m_Matrix;

    public F_Matrix(int i_Rows, int i_Columns)
    {
        m_Matrix = new int[i_Rows, i_Columns];
        for (int row = 0; row < Rows(); ++row)
        {
            for (int column = 0; column < Columns(); ++column)
            {
                Set(row, column, 0);
            }

        }
    }

    public void Set(int i_Row, int i_Column, int value)
    {
        m_Matrix[i_Row, i_Column] = value;
    }

    public int Get(int i_Row, int i_Column)
    {
        return m_Matrix[i_Row, i_Column];
    }

    public int Rows()
    {
        return m_Matrix.GetLength(0);
    }

    public int Columns()
    {
        return m_Matrix.GetLength(1);
    }

    public F_Matrix GetRotated(F_Direction i_Direction)
    {
        F_Matrix rotatedMatrix;

        int rows = m_Matrix.GetLength(0);
        int columns = m_Matrix.GetLength(1);

        switch (i_Direction)
        {
            case F_Direction.DOWN:
                rotatedMatrix = new F_Matrix(rows, columns);
                for(int row = 0; row < rows; ++row)
                {
                    for (int column = 0; column < columns; ++column)
                    {
                        rotatedMatrix.Set(row, column, Get(rows-1-row, columns -1 -column));
                    }

                }
                break;

            case F_Direction.RIGHT:
                rotatedMatrix = new F_Matrix(columns, rows);
                for (int row = 0; row < columns; ++row)
                {
                    for (int column = 0; column < rows; ++column)
                    {
                        rotatedMatrix.Set(row, column, Get(rows-1-column, row));
                    }

                }
                break;

            case F_Direction.LEFT:
                rotatedMatrix = new F_Matrix(columns, rows);
                for (int row = 0; row < columns; ++row)
                {
                    for (int column = 0; column < rows; ++column)
                    {
                        rotatedMatrix.Set(row, column, Get(column, columns-1-row));
                    }

                }
                break;

            default:
                rotatedMatrix = this;
                break;
        }

        return rotatedMatrix;
    }

    public void debugDisplay(string i_MatrixName)
    {
        string printMatrix = i_MatrixName+"\n";
        for (int row = 0; row < m_Matrix.GetLength(0); ++row)
        {
            for (int column = 0; column < m_Matrix.GetLength(1); ++column)
            {
                printMatrix += (Get(row, column))+" ";
            }
            printMatrix += "\n";

        }
        Debug.Log(printMatrix);
    }

}
