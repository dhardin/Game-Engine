using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Extensions
{
    public enum CardinalDirection
    {
        North = 0,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
        Unknown
    }

    public static class Vector2Extensions
    {
        public static CardinalDirection ToCardinalDirection(this Vector2 direction)
        {
            Vector2 resultant = direction;
            int xsign;
            int ysign;
            double arctan = Math.Atan2(resultant.Y, resultant.X);
            xsign = Math.Sign(resultant.X);
            ysign = Math.Sign(resultant.Y);
            switch (xsign)
            {
                case 0:
                case 1:
                    switch (ysign)
                    {
                        case 0:
                        case 1:
                            if (arctan >= (3 * MathHelper.Pi) / 8)
                            {
                                return CardinalDirection.North;
                            }
                            else if (arctan < MathHelper.Pi / 8)
                            {
                                return CardinalDirection.East;
                            }
                            else
                            {
                                return CardinalDirection.NorthEast;
                            }
                        case -1:
                            if (arctan <= ((-3) * MathHelper.Pi) / 8)
                            {
                                return CardinalDirection.South;
                            }
                            else if (arctan > (-MathHelper.Pi) / 8)
                            {
                                return CardinalDirection.East;

                            }
                            else
                            {
                                return CardinalDirection.SouthEast;
                            }
                    }
                    break;
                case -1:
                    switch (ysign)
                    {
                        case 0:
                        case 1:
                            if (arctan <= (5 * MathHelper.Pi) / 8)
                            {
                                return CardinalDirection.North;
                            }
                            else if (arctan >= (7 * MathHelper.Pi) / 8)
                            {
                                return CardinalDirection.West;
                            }
                            else
                            {
                                return CardinalDirection.NorthWest;
                            }

                        case -1:
                            if (arctan >= ((-5) * MathHelper.Pi) / 8)
                            {
                                return CardinalDirection.South;
                            }
                            else if (arctan < ((-9) * MathHelper.Pi) / 8)
                            {
                                return CardinalDirection.West;

                            }
                            else
                            {
                                return CardinalDirection.SouthWest;
                            }
                    }
                    break;

            }
            return CardinalDirection.Unknown;
        }
    }
}
