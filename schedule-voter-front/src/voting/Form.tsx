import './form.css'
import * as React from 'react'
import * as moment from 'moment'
import * as linq from 'linq'
import { Vote, Votes, voteToString, Voting } from './types'
import { VoteSelector } from './VoteSelector'

type CB<T> = (x: React.SetStateAction<T>) => void

export type Model = {
	isLoading: boolean
	viewResults: () => void
	dates: moment.Moment[]
	staticName: string
	onApprove: (votes: Votes) => void
	gw2User: string
	disUser: string
	votes: Voting[]
	setGw2User: CB<string>
	setDisUser: CB<string>
	setVotes: CB<Voting[]>
}

const dateFormat = "DD.MM.YYYY"

function DateButton({ voteCounts, onSelect, idx, date}: { date: string, voteCounts: { vote: Vote, count: number }[], idx: number, onSelect: (x: number) => void}) {
	const onClick = React.useCallback(() => onSelect(idx), [onSelect, idx])
	return (
		<div className="date-button" onClick={onClick}>
			<b className="date-button__header">{date}</b>
			<div className="date-button__votes">
				{voteCounts.map(v => <small key={v.vote}>{voteToString(v.vote)}: {v.count}</small>)}
			</div>
		</div>
	)
}

const gw2AccRegex = /^[\w\d\_\-]+\.\d\d\d\d$/
const disAccRegex = /^[\w\d\_\-]+\#\d\d\d\d$/

export function Form({ viewResults, dates, staticName, onApprove, isLoading, gw2User, disUser, votes, setGw2User, setDisUser, setVotes }: Model) {
	const onVote = React.useCallback((v: Voting) => setVotes(old => old.map(o => v.tourney.isSame(o.tourney) ? v : o)), [setVotes])

	const approve = React.useCallback(() => onApprove({ staticName, gw2Account: gw2User, disAccount: disUser, votes }), [gw2User, disUser, votes, staticName, onApprove])

	const [voteSelection, setVoteSelection] = React.useState<number | null>(null)
	const closeVotes = React.useCallback(() => setVoteSelection(null), [setVoteSelection])

	const groupedDates = React.useMemo(() => 
		linq.from(dates)
		.groupBy(
			d => moment.utc(d).local().startOf('day').format(dateFormat),
			x => x,
			(date, vs) => {
				const actualVotes = linq.from(votes).where(x => moment.utc(x.tourney).local().format(dateFormat) == date).orderBy(x => x.tourney).toArray()
				return {
					date,
					times: vs.select(x => ({ local: moment.utc(x).local().format("HH:mm"), original: x })).toArray(),
					actualVotes,
					voteCounts: linq.from([
						{ vote: Vote.CantAttend, count: 0 },
						{ vote: Vote.MayAttend, count: 0 },
						{ vote: Vote.WantAttend, count: 0 },
						...actualVotes.map(x => ({ vote: x.vote, count: 1}))
					]).groupBy(x => x.vote, x => x, (vote, els) => ({ vote, count: els.sum(x => x.count) })).toArray()
				}
			},
		)
		.where(x => x.times.length > 0)
		.orderBy(x => x.date)
		.toArray()
	, [dates, votes])

	const validationError = React.useMemo(() => {
		if (!gw2User)
			return 'Введи ГВшный акк, чумба'
		if (!gw2AccRegex.test(gw2User))
			return 'ГВшный акк - формата Choom.1234. Можешь посмотреть его в игре в френдлисте'
		if (!disUser)
			return 'Введи акк диса, чумба'
		if (!disAccRegex.test(disUser))
			return 'Акк диса - формата Choom#1234. Пишется в дисе слева внизу (на мобилке спрятан в левой шторке)'
		if (!votes.some(x => x.vote != Vote.CantAttend))
			return 'Если никуда идти не хочешь, что ты тут забыл, чумба?'
		if (isLoading)
			return 'Chill, choom, data is uploading'
		return false
	}, [gw2User, disUser, votes, isLoading])

	return (
		<div className="form">
			{voteSelection !== null && <VoteSelector selection={groupedDates[voteSelection]} onVote={onVote} onClose={closeVotes} /> || null}
			<h1>Расписание статика {staticName}</h1>
			<div className="submit-panel">
				<button type="button" className="submit-button" onClick={viewResults}>Посмотреть результаты</button>
			</div>
			<div className="account-inputs">
				<div className="account-name">
					<label>Аккаунт GW2:</label>
					<input type="text" placeholder="YourGW2AccName.1234" value={gw2User} onChange={e => setGw2User(e.target.value)}/>
				</div>
				<div className="account-name">
					<label>Аккаунт Discord:</label>
					<input type="text" placeholder="YourDiscordName#1234" value={disUser} onChange={e => setDisUser(e.target.value)}/>
				</div>
			</div>
			<div className="dates">
				{groupedDates.map((gd, idx) => <DateButton key={gd.date} date={gd.date} voteCounts={gd.voteCounts} idx={idx} onSelect={setVoteSelection} />)}
			</div>
			<div className="submit-panel">
				{validationError && <span className="validation-error">{validationError}</span> || <button type="button" className="submit-button" onClick={approve}>Отправить</button>}
			</div>
		</div>
	)
}