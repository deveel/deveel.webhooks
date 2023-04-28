#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.


using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Deveel.Webhooks.Facebook;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Deveel.Webhooks {
    public class FacebookWebhookTests : IDisposable {
		private readonly WebApplicationFactory<Program> appFactory;
		private FacebookWebhook? lastWebhook;

		public FacebookWebhookTests(ITestOutputHelper outputHelper) {
			appFactory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder => builder
					.ConfigureTestServices(ConfigureServices)
					.ConfigureLogging(logging => logging
						.AddXUnit(outputHelper, opt => opt.Filter = (cat, level) => true)
						.SetMinimumLevel(LogLevel.Trace)));
		}

		private void ConfigureServices(IServiceCollection services) {
			services.AddCallback<FacebookWebhook>(webhook => lastWebhook = webhook);
		}

		public void Dispose() => appFactory?.Dispose();

		private HttpClient CreateClient() => appFactory.CreateClient();

		private async Task<HttpResponseMessage> SendWebbookAsync(object content) {
			var client = CreateClient();
			return await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/webhooks/facebook") {
				Content = JsonContent.Create(content, content.GetType(), new MediaTypeHeaderValue("application/json"))
			});
		}

		[Fact]
		public async Task ReceiveDelivered() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.Equal("123456789", lastWebhook.Entries[0].Id);
			Assert.Equal(1458692752478, lastWebhook.Entries[0].TimeStamp.ToUnixTimeMilliseconds());
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
            Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
            Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Delivery);
            Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Delivery.MessageIds);
            Assert.NotEmpty(lastWebhook.Entries[0].Messaging[0].Delivery.MessageIds);
			Assert.Equal("mid.1458668856218:ed81099e15d3f4f233", lastWebhook.Entries[0].Messaging[0].Delivery.MessageIds[0]);
			Assert.Equal(1458668856253, lastWebhook.Entries[0].Messaging[0].Delivery.Watermark.ToUnixTimeMilliseconds());
		}

		[Fact]
		public async Task ReceiveRead() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.Equal("123456789", lastWebhook.Entries[0].Id);
			Assert.Equal(1458692752478, lastWebhook.Entries[0].TimeStamp.ToUnixTimeMilliseconds());
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Read);
			Assert.Equal(1458668856253, lastWebhook.Entries[0].Messaging[0].Read.Watermark.ToUnixTimeMilliseconds());
		}

		[Fact]
		public async Task ReceiveTextMessage() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Sender.Id);
			Assert.Equal("123456789", lastWebhook.Entries[0].Messaging[0].Sender.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Recipient.Id);
			Assert.Equal("987654321", lastWebhook.Entries[0].Messaging[0].Recipient.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Text);
			Assert.Equal("hello, world!", lastWebhook.Entries[0].Messaging[0].Message.Text);
		}

		[Fact]
		public async Task ReceiveMessageWithReplyTo() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Text);
			Assert.Equal("hello, world!", lastWebhook.Entries[0].Messaging[0].Message.Text);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.ReplyTo);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.ReplyTo?.Id);
			Assert.Equal("mid.1457764197618:41d102a3e1ae206a38", lastWebhook.Entries[0].Messaging[0].Message.ReplyTo?.Id);
		}

		[Fact]
		public async Task ReceiveFileAttachment() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.File, lastWebhook.Entries[0].Messaging[0].Message.Attachments[0].Type);
			
			var file = Assert.IsType<FileAttachment>(lastWebhook.Entries[0].Messaging[0].Message.Attachments[0]);
			Assert.NotNull(file.Payload);
			Assert.Equal("https://www.facebook.com/images/fb_icon_325x325.png", file.Payload.Url);
		}

		[Fact]
		public async Task ReceiveAudioAttachment() {						
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.Audio, lastWebhook.Entries[0].Messaging[0].Message.Attachments[0].Type);
			
			var audio = Assert.IsType<AudioAttachment>(lastWebhook.Entries[0].Messaging[0].Message.Attachments[0]);
			Assert.NotNull(audio.Payload);
			Assert.Equal("https://www.facebook.com/images/fb_icon_325x325.png", audio.Payload.Url);
		}

		[Fact]
		public async Task ReceiveProductTemplateWebhook() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.Template, lastWebhook.Entries[0].Messaging[0].Message.Attachments[0].Type);

			var template = Assert.IsType<TemplateAttachment>(lastWebhook.Entries[0].Messaging[0].Message.Attachments[0]);
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
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Id);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging[0].Message.Attachments);
			Assert.Equal(AttachmentType.Fallback, lastWebhook.Entries[0].Messaging[0].Message.Attachments[0].Type);

			var fallback = Assert.IsType<FallbackAttachment>(lastWebhook.Entries[0].Messaging[0].Message.Attachments[0]);
			Assert.NotNull(fallback.Payload);
			Assert.Equal("Callback", fallback.Payload.Title);
			Assert.Equal("https://www.facebook.com/images/fb_icon_325x325.png", fallback.Payload.Url);
		}

		[Fact]
		public async Task ReceiveOptInWebhook() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].OptIn);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.Equal("notification_messages", lastWebhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].OptIn.Payload);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].OptIn.NotificationToken);
			Assert.Equal(NotificationFequency.Daily, lastWebhook.Entries[0].Messaging[0].OptIn.Fequency);
			Assert.Equal("America/Los_Angeles", lastWebhook.Entries[0].Messaging[0].OptIn.Timezone);
			Assert.Equal(1458779227000, lastWebhook.Entries[0].Messaging[0].OptIn.TokenExpiresAt?.ToUnixTimeMilliseconds());
			Assert.Equal(UserTokenStatus.Refreshed, lastWebhook.Entries[0].Messaging[0].OptIn.UserTokenStatus);
		}

		[Fact]
		public async Task ReceiveOptOutWebhook() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].OptIn);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.Equal("notification_messages", lastWebhook.Entries[0].Messaging[0].OptIn.Type);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].OptIn.NotificationToken);
			Assert.Equal(NotificationStatus.Stop, lastWebhook.Entries[0].Messaging[0].OptIn.Status);
		}

		[Fact]
		public async Task ReceiveGamePlayWebhook() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].GamePlay);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].GamePlay.GameId);
			Assert.Equal("123456789", lastWebhook.Entries[0].Messaging[0].GamePlay.GameId);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].GamePlay.PlayerId);
			Assert.Equal("987654321", lastWebhook.Entries[0].Messaging[0].GamePlay.PlayerId);
			Assert.Equal(GameContextType.Thread, lastWebhook.Entries[0].Messaging[0].GamePlay.ContextType);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].GamePlay.ContextId);
			Assert.Equal("123456789", lastWebhook.Entries[0].Messaging[0].GamePlay.ContextId);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].GamePlay.Score);
			Assert.Equal(12, lastWebhook.Entries[0].Messaging[0].GamePlay.Score);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].GamePlay.Payload);
		}

		[Fact]
		public async Task ReceiveLikeReaction() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Reaction);
			Assert.Equal(ReactionType.Like, lastWebhook.Entries[0].Messaging[0].Reaction.Type);
			Assert.Equal(ReactionActionType.React, lastWebhook.Entries[0].Messaging[0].Reaction.Action);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.Equal("mid.1457764197618:41d102a3e1ae206a38", lastWebhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Reaction.Emoji);
			Assert.Equal("😍", lastWebhook.Entries[0].Messaging[0].Reaction.Emoji);
		}

		[Fact]
		public async Task ReceiveUnreactToLike() {
			var response = await SendWebbookAsync(new {
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

			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

			Assert.NotNull(lastWebhook);
			Assert.Equal("page", lastWebhook.Object);
			Assert.NotNull(lastWebhook.Entries);
			Assert.NotEmpty(lastWebhook.Entries);
			Assert.NotNull(lastWebhook.Entries[0].Messaging);
			Assert.NotEmpty(lastWebhook.Entries[0].Messaging);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Reaction);
			Assert.Equal(ReactionType.Like, lastWebhook.Entries[0].Messaging[0].Reaction.Type);
			Assert.Equal(ReactionActionType.Unreact, lastWebhook.Entries[0].Messaging[0].Reaction.Action);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.Equal("mid.1457764197618:41d102a3e1ae206a38", lastWebhook.Entries[0].Messaging[0].Reaction.MessageId);
			Assert.NotNull(lastWebhook.Entries[0].Messaging[0].Reaction.Emoji);
			Assert.Equal("😍", lastWebhook.Entries[0].Messaging[0].Reaction.Emoji);
		}

		[Fact]
		public async Task Verify_Success() {
			var verifyToken = appFactory.Services.GetRequiredService<IConfiguration>()["Facebook:VerifyToken"];

			var client = CreateClient();
			var response = await client.GetAsync($"/facebook/verify?hub.mode=subscribe&hub.challenge=1234567890&hub.verify_token={verifyToken}");
			Assert.True(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal("1234567890", await response.Content.ReadAsStringAsync());
		}

		[Fact]
		public async Task Verify_InvalidToken() {
			var client = CreateClient();
			var response = await client.GetAsync($"/facebook/verify?hub.mode=subscribe&hub.challenge=1234567890&hub.verify_token=invalid");
			
			Assert.False(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
		}

		[Fact]
		public async Task Verify_MissingToken() {
			var client = CreateClient();
			var response = await client.GetAsync($"/facebook/verify?hub.mode=subscribe&hub.challenge=1234567890");
			
			Assert.False(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Fact]
		public async Task Verify_MissingChallenge() {
			var client = CreateClient();
			var response = await client.GetAsync($"/facebook/verify?hub.mode=subscribe&hub.verify_token=1234567890");
			
			Assert.False(response.IsSuccessStatusCode);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}
