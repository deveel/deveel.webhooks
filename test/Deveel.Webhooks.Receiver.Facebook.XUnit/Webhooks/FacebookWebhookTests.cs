#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.


using System.Net;
using System.Text.Json;

using Deveel.Webhooks.Facebook;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
	public class FacebookWebhookTests {
		public FacebookWebhookTests(ITestOutputHelper outputHelper) {
			var services = new ServiceCollection();

			ConfigureServices(services, outputHelper);

			Services = services.BuildServiceProvider();
		}

		private IServiceProvider Services { get; }

		private void ConfigureServices(IServiceCollection services, ITestOutputHelper outputHelper) {
			services.AddLogging(logging => logging.AddXUnit(outputHelper).SetMinimumLevel(LogLevel.Trace));

			services.AddFacebookReceiver(options => {
				options.VerifyToken = "9488500595995";
				options.AppSecret = "hv3OkdL111_3lj";
			});
		}

		private async Task<WebhookReceiveResult<FacebookWebhook>> ReceiveWebhookAsync(object content) {
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers["Host"] = "example.com";
			httpContext.Request.Scheme = "https";
			httpContext.Request.Method = "POST";
			httpContext.Request.Path = "/webhooks/facebook";
			httpContext.Request.ContentType = "application/json";
			using var memory = new MemoryStream();
			await JsonSerializer.SerializeAsync(memory, content, content.GetType());
			memory.Position = 0;
			httpContext.Request.Body = memory;

			var receiver = Services.GetRequiredService<IWebhookReceiver<FacebookWebhook>>();
			return await receiver.ReceiveAsync(httpContext.Request);
		}

		[Fact]
		public async Task ReceiveDelivered() {
			var result = await ReceiveWebhookAsync(new {
				@object = "page",
				entry = new[] {
					new {
						id = "123456789",
						time = 1458692752478,
						messaging = new[] {
							new {
								sender = new {
									id = "123456789"
								},
								recipient = new {
									id = "987654321"
								},
								delivery = new {
									mids = new[] {
										"mid.1458668856218:ed81099e15d3f4f233"
									},
									watermark = 1458668856253,
									seq = 37
								}
							}
						}
					}
				}
			});

			Assert.True(result.Successful);
			Assert.NotNull(result.Webhook);

			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.Equal("123456789", result.Webhook.Entries[0].Id);
			Assert.Equal(1458692752478, result.Webhook.Entries[0].TimeStamp.ToUnixTimeMilliseconds());
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Delivery);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Delivery.MessageIds);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging[0].Delivery.MessageIds);
			Assert.Equal("mid.1458668856218:ed81099e15d3f4f233", result.Webhook.Entries[0].Messaging[0].Delivery.MessageIds[0]);
			Assert.Equal(1458668856253, result.Webhook.Entries[0].Messaging[0].Delivery.Watermark.ToUnixTimeMilliseconds());
		}

		[Fact]
		public async Task ReceiveRead() {
			var result = await ReceiveWebhookAsync(new {
				@object = "page",
				entry = new[] {
					new {
						id = "123456789",
						time = 1458692752478,
						messaging = new[] {
							new {
								sender = new {
									id = "123456789"
								},
								recipient = new {
									id = "987654321"
								},
								read = new {
									watermark = 1458668856253,
									seq = 37
								}
							}
						}
					}
				}
			});

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.Equal("123456789", result.Webhook.Entries[0].Id);
			Assert.Equal(1458692752478, result.Webhook.Entries[0].TimeStamp.ToUnixTimeMilliseconds());
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Read);
			Assert.Equal(1458668856253, result.Webhook.Entries[0].Messaging[0].Read.Watermark.ToUnixTimeMilliseconds());
		}

		[Fact]
		public async Task ReceiveTextMessage() {
			var result = await ReceiveWebhookAsync(new {
				@object = "page",
				entry = new[] {
					new {
						id = "123456789",
						time = 1458692752478,
						messaging = new[] {
							new {
								sender = new {
									id = "123456789"
								},
								recipient = new {
									id = "987654321"
								},
								timestamp = 1458692752478,
								message = new {
									mid = "mid.1457764197618:41d102a3e1ae206a38",
									seq = 73,
									text = "hello, world!"
								}
							}
						}
					}
				}
			});

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Sender.Id);
			Assert.Equal("123456789", result.Webhook.Entries[0].Messaging[0].Sender.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Recipient.Id);
			Assert.Equal("987654321", result.Webhook.Entries[0].Messaging[0].Recipient.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Text);
			Assert.Equal("hello, world!", result.Webhook.Entries[0].Messaging[0].Message.Text);
		}

		[Fact]
		public async Task ReceiveMessageWithReplyTo() {
			var result = await ReceiveWebhookAsync(new {
				@object = "page",
				entry = new[] {
					new {
						id = "123456789",
						time = 1458692752478,
						messaging = new[] {
							new {
								sender = new {
									id = "123456789"
								},
								recipient = new {
									id = "987654321"
								},
								timestamp = 1458692752478,
								message = new {
									mid = "mid.1457764197618:41d102a3e1ae206a38",
									seq = 73,
									text = "hello, world!",
									reply_to = new {
										mid = "mid.1457764197618:41d102a3e1ae206a38"
									}
								}
							}
						}
					}
				}
			});

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Text);
			Assert.Equal("hello, world!", result.Webhook.Entries[0].Messaging[0].Message.Text);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.ReplyTo);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.ReplyTo?.Id);
			Assert.Equal("mid.1457764197618:41d102a3e1ae206a38", result.Webhook.Entries[0].Messaging[0].Message.ReplyTo?.Id);
		}

		[Fact]
		public async Task ReceiveFileAttachment() {
			var result = await ReceiveWebhookAsync(new {
				@object = "page",
				entry = new[] {
					new {
						id = "123456789",
						time = 1458692752478,
						messaging = new[] {
							new {
								sender = new {
									id = "123456789"
								},
								recipient = new {
									id = "987654321"
								},
								timestamp = 1458692752478,
								message = new {
									mid = "mid.1457764197618:41d102a3e1ae206a38",
									seq = 73,
									attachments = new[] {
										new {
											type = "file",
											payload = new {
												url = "https://www.facebook.com/images/fb_icon_325x325.png"
											}
										}
									}
								}
							}
						}
					}
				}
			});

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.File, result.Webhook.Entries[0].Messaging[0].Message.Attachments[0].Type);
			
			var file = Assert.IsType<FileAttachment>(result.Webhook.Entries[0].Messaging[0].Message.Attachments[0]);
			Assert.NotNull(file.Payload);
			Assert.Equal("https://www.facebook.com/images/fb_icon_325x325.png", file.Payload.Url);
		}

		[Fact]
		public async Task ReceiveAudioAttachment() {						
			var result = await ReceiveWebhookAsync(new {
				@object = "page",
				entry = new[] {
					new {
						id = "123456789",
						time = 1458692752478,
						messaging = new[] {
							new {
								sender = new {
									id = "123456789"
								},
								recipient = new {
									id = "987654321"
								},
								timestamp = 1458692752478,
								message = new {
									mid = "mid.1457764197618:41d102a3e1ae206a38",
									seq = 73,
									attachments = new[] {
										new {
											type = "audio",
											payload = new {
												url = "https://www.facebook.com/images/fb_icon_325x325.png"
											}
										}
									}
								}
							}
						}
					}
				}
			});

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.Audio, result.Webhook.Entries[0].Messaging[0].Message.Attachments[0].Type);
			
			var audio = Assert.IsType<AudioAttachment>(result.Webhook.Entries[0].Messaging[0].Message.Attachments[0]);
			Assert.NotNull(audio.Payload);
			Assert.Equal("https://www.facebook.com/images/fb_icon_325x325.png", audio.Payload.Url);
		}

		[Fact]
		public async Task ReceiveProductTemplateWebhook() {
			var result = await ReceiveWebhookAsync(new {
				@object = "page",
				entry = new[] {
					new {
						id = "123456789",
						time = 1458692752478,
						messaging = new[] {
							new {
								sender = new {
									id = "123456789"
								},
								recipient = new {
									id = "987654321"
								},
								timestamp = 1458692752478,
								message = new {
									mid = "mid.1457764197618:41d102a3e1ae206a38",
									seq = 73,
									attachments = new[] {
										new {
											type = "template",
											payload = new {
												product = new {
													elements = new object[] {
														new {
															id = "123456789",
															title = "Product title",
															subtitle = "Product subtitle",
															image_url = "https://www.facebook.com/images/fb_icon_325x325.png",
															retailer_id = "abc123456789",
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			});

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.Template, result.Webhook.Entries[0].Messaging[0].Message.Attachments[0].Type);

			var template = Assert.IsType<TemplateAttachment>(result.Webhook.Entries[0].Messaging[0].Message.Attachments[0]);
			Assert.NotNull(template.Payload);
			Assert.NotNull(template.Payload.Product);
			Assert.NotNull(template.Payload.Product.Elements);
			Assert.NotEmpty(template.Payload.Product.Elements);
			Assert.Equal("123456789", template.Payload.Product.Elements[0].Id);
			Assert.Equal("Product title", template.Payload.Product.Elements[0].Title);
			Assert.Equal("Product subtitle", template.Payload.Product.Elements[0].Subtitle);
			Assert.Equal("https://www.facebook.com/images/fb_icon_325x325.png", template.Payload.Product.Elements[0].ImageUrl);
			Assert.Equal("abc123456789", template.Payload.Product.Elements[0].RetailerId);
		}

		[Fact]
		public async Task ReceiveFallbackAttachment() {
			var result = await ReceiveWebhookAsync(new {
                @object = "page",
                entry = new[] {
                    new {
                        id = "123456789",
                        time = 1458692752478,
                        messaging = new[] {
                            new {
                                sender = new {
                                    id = "123456789"
                                },
                                recipient = new {
                                    id = "987654321"
                                },
                                timestamp = 1458692752478,
                                message = new {
                                    mid = "mid.1457764197618:41d102a3e1ae206a38",
                                    seq = 73,
                                    attachments = new[] {
                                        new {
                                            type = "fallback",
                                            payload = new {
												title = "Callback",
                                                url = "https://www.facebook.com/images/fb_icon_325x325.png"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.Fallback, result.Webhook.Entries[0].Messaging[0].Message.Attachments[0].Type);

			var fallback = Assert.IsType<FallbackAttachment>(result.Webhook.Entries[0].Messaging[0].Message.Attachments[0]);
			Assert.NotNull(fallback.Payload);
			Assert.Equal("Callback", fallback.Payload.Title);
			Assert.Equal("https://www.facebook.com/images/fb_icon_325x325.png", fallback.Payload.Url);
		}

		[Fact]
		public async Task ReceiveOptInWebhook() {
			var result = await ReceiveWebhookAsync(new {
                @object = "page",
                entry = new[] {
                    new {
                        id = "123456789",
                        time = 1458692752478,
                        messaging = new[] {
                            new {
                                sender = new {
                                    id = "123456789"
                                },
                                recipient = new {
                                    id = "987654321"
                                },
                                timestamp = 1458692752478,
                                optin = new {
                                    type = "notification_messages",
									payload = "USER_DEFINED_PAYLOAD",
									notification_messages_token = "NOTIFICATION_MESSAGES_TOKEN",
									notification_messages_frequency = "DAILY",
									notification_messages_timezone = "America/Los_Angeles",
									token_expiry_timestamp = 1458779227000,
									user_token_status = "REFRESHED"
                                }
                            }
                        }
                    }
                }
            });

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].OptIn);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.Equal("notification_messages", result.Webhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].OptIn.Payload);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].OptIn.NotificationToken);
			Assert.Equal(NotificationFequency.Daily, result.Webhook.Entries[0].Messaging[0].OptIn.Fequency);
			Assert.Equal("America/Los_Angeles", result.Webhook.Entries[0].Messaging[0].OptIn.Timezone);
			Assert.Equal(1458779227000, result.Webhook.Entries[0].Messaging[0].OptIn.TokenExpiresAt?.ToUnixTimeMilliseconds());
			Assert.Equal(UserTokenStatus.Refreshed, result.Webhook.Entries[0].Messaging[0].OptIn.UserTokenStatus);
		}

		[Fact]
		public async Task ReceiveOptOutWebhook() {
			var result = await ReceiveWebhookAsync(new {
                @object = "page",
                entry = new[] {
                    new {
                        id = "123456789",
                        time = 1458692752478,
                        messaging = new[] {
                            new {
                                sender = new {
                                    id = "123456789"
                                },
                                recipient = new {
                                    id = "987654321"
                                },
                                timestamp = 1458692752478,
                                optin = new {
									type = "notification_messages",
									notification_messages_token = "NOTIFICATION_MESSAGES_TOKEN",
									notification_messages_status = "STOP NOTIFICATIONS"
                                }
                            }
                        }
                    }
                }
            });

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].OptIn);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.Equal("notification_messages", result.Webhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].OptIn.NotificationToken);
			Assert.Equal(NotificationStatus.Stop, result.Webhook.Entries[0].Messaging[0].OptIn.Status);
		}

		[Fact]
		public async Task ReceiveGamePlayWebhook() {
			var result = await ReceiveWebhookAsync(new {
                @object = "page",
                entry = new[] {
                    new {
                        id = "123456789",
                        time = 1458692752478,
                        messaging = new[] {
                            new {
                                sender = new {
                                    id = "123456789"
                                },
                                recipient = new {
                                    id = "987654321"
                                },
                                timestamp = 1458692752478,
                                game_play = new {
                                    game_id = "123456789",
                                    player_id = "987654321",
                                    context_type = "THREAD",
                                    context_id = "123456789",
                                    score = 12,
                                    payload = new {
										my_thing = 22,
										their = "Thing"
									}
                                }
                            }
                        }
                    }
                }
            });

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].GamePlay);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].GamePlay.GameId);
			Assert.Equal("123456789", result.Webhook.Entries[0].Messaging[0].GamePlay.GameId);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].GamePlay.PlayerId);
			Assert.Equal("987654321", result.Webhook.Entries[0].Messaging[0].GamePlay.PlayerId);
			Assert.Equal(GameContextType.Thread, result.Webhook.Entries[0].Messaging[0].GamePlay.ContextType);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].GamePlay.ContextId);
			Assert.Equal("123456789", result.Webhook.Entries[0].Messaging[0].GamePlay.ContextId);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].GamePlay.Score);
			Assert.Equal(12, result.Webhook.Entries[0].Messaging[0].GamePlay.Score);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].GamePlay.Payload);
		}

		[Fact]
		public async Task ReceiveLikeReaction() {
			var result = await ReceiveWebhookAsync(new {
                @object = "page",
                entry = new[] {
                    new {
                        id = "123456789",
                        time = 1458692752478,
                        messaging = new[] {
                            new {
                                sender = new {
                                    id = "123456789"
                                },
                                recipient = new {
                                    id = "987654321"
                                },
                                timestamp = 1458692752478,
                                reaction = new {
                                    reaction = "like",
									action = "react",
									mid = "mid.1457764197618:41d102a3e1ae206a38",
                                    emoji = "😍"
                                }
                            }
                        }
                    }
                }
            });

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Reaction);
			Assert.Equal(ReactionType.Like, result.Webhook.Entries[0].Messaging[0].Reaction.Type);
			Assert.Equal(ReactionActionType.React, result.Webhook.Entries[0].Messaging[0].Reaction.Action);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.Equal("mid.1457764197618:41d102a3e1ae206a38", result.Webhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Reaction.Emoji);
			Assert.Equal("😍", result.Webhook.Entries[0].Messaging[0].Reaction.Emoji);
		}

		[Fact]
		public async Task ReceiveUnreactToLike() {
			var result = await ReceiveWebhookAsync(new {
                @object = "page",
                entry = new[] {
                    new {
                        id = "123456789",
                        time = 1458692752478,
                        messaging = new[] {
                            new {
                                sender = new {
                                    id = "123456789"
                                },
                                recipient = new {
                                    id = "987654321"
                                },
                                timestamp = 1458692752478,
                                reaction = new {
                                    reaction = "like",
                                    action = "unreact",
                                    mid = "mid.1457764197618:41d102a3e1ae206a38",
                                    emoji = "😍"
                                }
                            }
                        }
                    }
                }
            });

			Assert.True(result.Successful);

			Assert.NotNull(result.Webhook);
			Assert.Equal("page", result.Webhook.Object);
			Assert.NotNull(result.Webhook.Entries);
			Assert.NotEmpty(result.Webhook.Entries);
			Assert.NotNull(result.Webhook.Entries[0].Messaging);
			Assert.NotEmpty(result.Webhook.Entries[0].Messaging);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Reaction);
			Assert.Equal(ReactionType.Like, result.Webhook.Entries[0].Messaging[0].Reaction.Type);
			Assert.Equal(ReactionActionType.Unreact, result.Webhook.Entries[0].Messaging[0].Reaction.Action);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.Equal("mid.1457764197618:41d102a3e1ae206a38", result.Webhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.NotNull(result.Webhook.Entries[0].Messaging[0].Reaction.Emoji);
			Assert.Equal("😍", result.Webhook.Entries[0].Messaging[0].Reaction.Emoji);
		}

		private HttpContext CreateVerifyContext(string? token = null, string? challenge = null) {
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Method = "GET";
			httpContext.Request.Path = "/facebook/verify";
			var queryString = new List<string> {
				"hub.mode=subscribe"
			};
			if (!String.IsNullOrWhiteSpace(token))
				queryString.Add($"hub.verify_token={token}");
			if (!String.IsNullOrWhiteSpace(challenge))
				queryString.Add($"hub.challenge={challenge}");

			httpContext.Request.QueryString = new QueryString($"?{String.Join("&", queryString)}");
			httpContext.Response.Body = new MemoryStream();
			return httpContext;
		}

		private async Task<IWebhookVerificationResult> VerifyAsync(string? token = null, string? challenge = null) {
			var httpContext = CreateVerifyContext(token, challenge);

			var verifier = Services.GetRequiredService<IWebhookRequestVerifier<FacebookWebhook>>();
			return await verifier.VerifyRequestAsync(httpContext.Request, default);
		}

		[Fact]
		public async Task Verify_Success() {
			var verifier = Services.GetRequiredService<IWebhookRequestVerifier<FacebookWebhook>>();

			var httpContext = CreateVerifyContext("9488500595995", "1234567890");
			var result = await verifier.VerifyRequestAsync(httpContext.Request);

			Assert.True(result.IsValid);
			Assert.True(result.IsVerified);

			var fbResult = Assert.IsType<FacebookVerificationResult>(result);
			Assert.Equal("1234567890", fbResult.Challenge);

			await verifier.HandleResultAsync(fbResult, httpContext.Response, default);

			Assert.Equal(200, httpContext.Response.StatusCode);
			Assert.Equal("text/plain", httpContext.Response.ContentType);

			httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

			using var reader = new StreamReader(httpContext.Response.Body);
			var challenge = await reader.ReadToEndAsync();

			Assert.Equal("1234567890", challenge);
		}

		[Fact]
		public async Task Verify_InvalidToken() {
			var result = await VerifyAsync("invalid", "1234567890");

			Assert.False(result.IsValid);
			Assert.True(result.IsVerified);
		}

		[Fact]
		public async Task Verify_MissingToken() {
			var result = await VerifyAsync(challenge: "1234567890");

			Assert.False(result.IsValid);
			Assert.False(result.IsVerified);
		}

		[Fact]
		public async Task Verify_MissingChallenge() {
			var result = await VerifyAsync("9488500595995");

			Assert.False(result.IsValid);
			Assert.False(result.IsVerified);
		}
	}
}
