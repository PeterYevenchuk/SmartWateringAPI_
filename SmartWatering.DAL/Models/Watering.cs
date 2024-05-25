﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWatering.DAL.Models;

public class Watering
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string SprinklerNameId { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }
}